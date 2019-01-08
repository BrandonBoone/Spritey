namespace Spritey.ImageProcessing.CompositeMapping
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Maps images Optimaly
    /// </summary>
    /// <typeparam name="T">A class that implements <see cref="ISprite"/></typeparam>
    internal class MapperOptimalEfficiency<T> : MapperOptimalEfficiency_Base<T>
        where T : class, ISprite, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperOptimalEfficiency{S}"/> class.
        /// See MapperIterative_Base
        /// </summary>
        /// <param name="canvas">
        /// Canvas to be used by the Mapping method.
        /// </param>
        internal MapperOptimalEfficiency(ICanvas canvas)
            : base(canvas)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperOptimalEfficiency{S}"/> class.
        /// See MapperIterative_Base
        /// </summary>
        /// <param name="canvas">
        /// Canvas to be used by the Mapping method.
        /// </param>
        /// <param name="cutoffEfficiency">
        /// The Mapping method will stop trying to get a better solution once it has found a solution
        /// with this efficiency.
        /// </param>
        /// <param name="maxNbrCandidateSprites">
        /// The Mapping method will stop trying to get a better solution once it has generated this many
        /// candidate sprites.
        /// </param>
        internal MapperOptimalEfficiency(ICanvas canvas, float cutoffEfficiency, int maxNbrCandidateSprites)
            : base(canvas, cutoffEfficiency, maxNbrCandidateSprites)
        {
        }

        /// <inheritdoc/>
        public override T Mapping(IEnumerable<IImageInfo> images, IMapperStats mapperStats)
        {
            int candidateSpriteFails = 0;
            int candidateSpritesGenerated = 0;
            int canvasRectangleAddAttempts = 0;
            int canvasNbrCellsGenerated = 0;

            // Sort the images by height descending
            IOrderedEnumerable<IImageInfo> imageInfosHighestFirst =
                images.OrderByDescending(p => p.Height);

            int totalAreaAllImages =
                (from a in imageInfosHighestFirst select a.Width * a.Height).Sum();

            int widthWidestImage =
                (from a in imageInfosHighestFirst select a.Width).Max();

            int heightHighestImage = imageInfosHighestFirst.First().Height;

            T bestSprite = null;

            int canvasMaxWidth = this.Canvas.UnlimitedSize;
            int canvasMaxHeight = heightHighestImage;

            while (canvasMaxWidth >= widthWidestImage)
            {
                var canvasStats = new CanvasStats();
                T spriteInfo =
                    this.MappingRestrictedBox(imageInfosHighestFirst, canvasMaxWidth, canvasMaxHeight, canvasStats, out int lowestFreeHeightDeficitTallestRightFlushedImage);

                canvasRectangleAddAttempts += canvasStats.RectangleAddAttempts;
                canvasNbrCellsGenerated += canvasStats.NbrCellsGenerated;

                if (spriteInfo == null)
                {
                    // Failure - Couldn't generate a SpriteInfo with the given maximum canvas dimensions
                    candidateSpriteFails++;

                    // Try again with a greater max height. Add enough height so that
                    // you don't get the same rectangle placement as this time.
                    if (canvasStats.LowestFreeHeightDeficit == int.MaxValue)
                    {
                        canvasMaxHeight++;
                    }
                    else
                    {
                        canvasMaxHeight += canvasStats.LowestFreeHeightDeficit;
                    }
                }
                else
                {
                    // Success - Managed to generate a SpriteInfo with the given maximum canvas dimensions
                    candidateSpritesGenerated++;

                    // Find out if the new SpriteInfo is better than the current best one
                    if ((bestSprite == null) || (bestSprite.Area > spriteInfo.Area))
                    {
                        bestSprite = spriteInfo;

                        float bestEfficiency = (float)totalAreaAllImages / spriteInfo.Area;
                        if (bestEfficiency >= this.CutoffEfficiency)
                        {
                            break;
                        }
                    }

                    if (candidateSpritesGenerated >= this.MaxNbrCandidateSprites)
                    {
                        break;
                    }

                    // Try again with a reduce maximum canvas width, to see if we can squeeze out a smaller sprite
                    // Note that in this algorithm, the maximum canvas width is never increased, so a new sprite
                    // always has the same or a lower width than an older sprite.
                    canvasMaxWidth = bestSprite.Width - 1;

                    // Now that we've decreased the width of the canvas to 1 pixel less than the width
                    // taken by the images on the canvas, we know for sure that the images whose
                    // right borders are most to the right will have to move up.
                    //
                    // To make sure that the next try is not automatically a failure, increase the height of the
                    // canvas sufficiently for the tallest right flushed image to be placed. Note that when
                    // images are placed sorted by highest first, it will be the tallest right flushed image
                    // that will fail to be placed if we don't increase the height of the canvas sufficiently.
                    if (lowestFreeHeightDeficitTallestRightFlushedImage == int.MaxValue)
                    {
                        canvasMaxHeight++;
                    }
                    else
                    {
                        canvasMaxHeight += lowestFreeHeightDeficitTallestRightFlushedImage;
                    }
                }

                // ---------------------
                // Adjust max canvas width and height to cut out sprites that we'll never accept
                int bestSpriteArea = bestSprite.Area;

                while (
                    (canvasMaxWidth >= widthWidestImage) &&
                    (!this.CandidateCanvasFeasable(
                            canvasMaxWidth,
                            canvasMaxHeight,
                            bestSpriteArea,
                            totalAreaAllImages,
                            out bool candidateBiggerThanBestSprite,
                            out bool candidateSmallerThanCombinedImages)))
                {
                    if (candidateBiggerThanBestSprite)
                    {
                        canvasMaxWidth--;
                    }

                    if (candidateSmallerThanCombinedImages)
                    {
                        canvasMaxHeight++;
                    }
                }
            }

            if (mapperStats != null)
            {
                mapperStats.CandidateSpriteFails = candidateSpriteFails;
                mapperStats.CandidateSpritesGenerated = candidateSpritesGenerated;
                mapperStats.CanvasNbrCellsGenerated = canvasNbrCellsGenerated;
                mapperStats.CanvasRectangleAddAttempts = canvasRectangleAddAttempts;
            }

            return bestSprite;
        }

        /// <summary>
        /// Works out whether there is any point in trying to fit the images on a canvas
        /// with the given width and height.
        /// </summary>
        /// <param name="canvasMaxWidth">Candidate canvas width</param>
        /// <param name="canvasMaxHeight">Candidate canvas height</param>
        /// <param name="bestSpriteArea">Area of the smallest sprite produces so far</param>
        /// <param name="totalAreaAllImages">Total area of all images</param>
        /// <param name="candidateBiggerThanBestSprite">true if the candidate canvas is bigger than the best sprite so far</param>
        /// <param name="candidateSmallerThanCombinedImages">true if the candidate canvas is smaller than the combined images</param>
        /// <returns>true, the images will fit on the canvas. false, they cannot</returns>
        protected virtual bool CandidateCanvasFeasable(
            int canvasMaxWidth,
            int canvasMaxHeight,
            int bestSpriteArea,
            int totalAreaAllImages,
            out bool candidateBiggerThanBestSprite,
            out bool candidateSmallerThanCombinedImages)
        {
            int candidateArea = canvasMaxWidth * canvasMaxHeight;
            candidateBiggerThanBestSprite = candidateArea > bestSpriteArea;
            candidateSmallerThanCombinedImages = candidateArea < totalAreaAllImages;

            return !(candidateBiggerThanBestSprite || candidateSmallerThanCombinedImages);
        }
    }
}
