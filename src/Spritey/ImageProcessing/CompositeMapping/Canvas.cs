#pragma warning disable SA1614 // Element parameter documentation should have text. Not my original code, suppressing for now.
namespace Spritey.ImageProcessing.CompositeMapping
{
    /// <summary>
    /// This type of canvas places rectangles as far to the left as possible (lowest X).
    /// If there is a choice between locations with the same X, it will pick the one with the
    /// lowest Y.
    /// </summary>
    internal class Canvas : ICanvas
    {
        private int canvasWidth = 0;
        private int canvasHeight = 0;

        // Lowest free height deficit found since the last call to SetCanvasDimension
        private int lowestFreeHeightDeficitSinceLastRedim = int.MaxValue;

        private int nbrCellsGenerated = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class.
        /// </summary>
        internal Canvas()
        {
        }

        /// <summary>
        /// Gets the number of rectangle add attempts
        /// </summary>
        public int NbrRectangleAddAttempts { get; private set; } = 0;

        /// <inheritdoc/>
        public int UnlimitedSize
        {
            get { return short.MaxValue; }
        }

        /// <summary>
        /// Gets _canvasCells to make it available to canvas classes derived from this class.
        /// </summary>
        protected DynamicTwoDimensionalArray<CanvasCell> CanvasCells { get; } = new DynamicTwoDimensionalArray<CanvasCell>();

        /// <inheritdoc/>
        public void GetStatistics(ICanvasStats canvasStats)
        {
            canvasStats.NbrCellsGenerated = this.nbrCellsGenerated;
            canvasStats.RectangleAddAttempts = this.NbrRectangleAddAttempts;
            canvasStats.LowestFreeHeightDeficit = this.lowestFreeHeightDeficitSinceLastRedim;
        }

        /// <inheritdoc/>
        public virtual void SetCanvasDimensions(int canvasWidth, int canvasHeight)
        {
            // Right now, it is unknown how many rectangles need to be placed.
            // So guess that a 100 by 100 capacity will be enough.
            const int initialCapacityX = 100;
            const int initialCapacityY = 100;

            // Initially, there is one free cell, which covers the entire canvas.
            this.CanvasCells.Initialize(initialCapacityX, initialCapacityY, canvasWidth, canvasHeight, new CanvasCell(false));

            this.nbrCellsGenerated = 0;
            this.NbrRectangleAddAttempts = 0;
            this.lowestFreeHeightDeficitSinceLastRedim = int.MaxValue;

            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
        }

        /// <inheritdoc/>
        public virtual bool AddRectangle(
            int rectangleWidth,
            int rectangleHeight,
            out int rectangleXOffset,
            out int rectangleYOffset,
            out int lowestFreeHeightDeficit)
        {
            rectangleXOffset = 0;
            rectangleYOffset = 0;
            lowestFreeHeightDeficit = int.MaxValue;

            int requiredWidth = rectangleWidth;
            int requiredHeight = rectangleHeight;

            this.NbrRectangleAddAttempts++;

            int x = 0;
            int y = 0;
            int offsetX = 0;
            int offsetY = 0;
            bool rectangleWasPlaced = false;
            int nbrRows = this.CanvasCells.NbrRows;

            do
            {
                // First move upwards until we find an unoccupied cell.
                // If we're already at an unoccupied cell, no need to do anything.
                // Important to clear all occupied cells to get
                // the lowest free height deficit. This must be taken from the top of the highest
                // occupied cell.
                while ((y < nbrRows) && this.CanvasCells.Item(x, y).Occupied)
                {
                    offsetY += this.CanvasCells.RowHeight(y);
                    y += 1;
                }

                // If we found an unoccupied cell, than see if we can place a rectangle there.
                // If not, than y popped out of the top of the canvas.
                if ((y < nbrRows) && (this.FreeHeightDeficit(this.canvasHeight, offsetY, requiredHeight) <= 0))
                {
                    if (this.IsAvailable(
                        x,
                        y,
                        requiredWidth,
                        requiredHeight,
                        out int nbrRequiredCellsHorizontally,
                        out int nbrRequiredCellsVertically,
                        out int leftOverWidth,
                        out int leftOverHeight))
                    {
                        this.PlaceRectangle(
                            x,
                            y,
                            requiredWidth,
                            requiredHeight,
                            nbrRequiredCellsHorizontally,
                            nbrRequiredCellsVertically,
                            leftOverWidth,
                            leftOverHeight);

                        rectangleXOffset = offsetX;
                        rectangleYOffset = offsetY;

                        rectangleWasPlaced = true;
                        break;
                    }

                    // Go to the next cell
                    offsetY += this.CanvasCells.RowHeight(y);
                    y += 1;
                }

                // If we've come so close to the top of the canvas that there is no space for the
                // rectangle, go to the next column. This automatically also checks whether we've popped out of the top
                // of the canvas (in that case, _canvasHeight == offsetY).
                int freeHeightDeficit = this.FreeHeightDeficit(this.canvasHeight, offsetY, requiredHeight);
                if (freeHeightDeficit > 0)
                {
                    offsetY = 0;
                    y = 0;

                    offsetX += this.CanvasCells.ColumnWidth(x);
                    x += 1;

                    // This update is far from perfect, because if the rectangle could not be placed at this column
                    // because of insufficient horizontal space, than this update should not be made (because it may lower
                    // _lowestFreeHeightDeficitSinceLastRedim while in raising the height of the canvas by this lowered amount
                    // may not result in the rectangle being placed here after all.
                    //
                    // However, checking for sufficient horizontal width takes a lot of CPU ticks. Tests have shown that this
                    // far outstrips the gains through having fewer failed sprite generations.
                    if (this.lowestFreeHeightDeficitSinceLastRedim > freeHeightDeficit)
                    {
                        this.lowestFreeHeightDeficitSinceLastRedim = freeHeightDeficit;
                    }
                }

                // If we've come so close to the right edge of the canvas that there is no space for
                // the rectangle, return false now.
                if ((this.canvasWidth - offsetX) < requiredWidth)
                {
                    rectangleWasPlaced = false;
                    break;
                }
            } while (true);

            lowestFreeHeightDeficit = this.lowestFreeHeightDeficitSinceLastRedim;
            return rectangleWasPlaced;
        }

        /// <summary>
        /// Works out the free height deficit when placing a rectangle with a required height at a given offset.
        ///
        /// If the free height deficit is 0 or negative, there may be room to place the rectangle (still need to check for blocking
        /// occupied cells).
        ///
        /// If the free height deficit is greater than 0, you're too close to the top edge of the canvas to place the rectangle.
        /// </summary>
        /// <param name="canvasHeight"></param>
        /// <param name="offsetY"></param>
        /// <param name="requiredHeight"></param>
        /// <returns>The free height deficit</returns>
        private int FreeHeightDeficit(int canvasHeight, int offsetY, int requiredHeight)
        {
            int spaceLeftVertically = canvasHeight - offsetY;
            int freeHeightDeficit = requiredHeight - spaceLeftVertically;

            return freeHeightDeficit;
        }

        /// <summary>
        /// Sets the cell at x,y to occupied, and also its top and right neighbours, as needed
        /// to place a rectangle with the given width and height.
        ///
        /// If the rectangle takes only part of a row or column, they are split.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="requiredWidth"></param>
        /// <param name="requiredHeight"></param>
        /// <param name="nbrRequiredCellsHorizontally">
        /// Number of cells that the rectangle requires horizontally
        /// </param>
        /// <param name="nbrRequiredCellsVertically">
        /// Number of cells that the rectangle requires vertically
        /// </param>
        /// <param name="leftOverWidth">
        /// The amount of horizontal space left in the right most cells that could be used for the rectangle
        /// </param>
        /// <param name="leftOverHeight">
        /// The amount of vertical space left in the bottom most cells that could be used for the rectangle
        /// </param>
        private void PlaceRectangle(
            int x,
            int y,
            int requiredWidth,
            int requiredHeight,
            int nbrRequiredCellsHorizontally,
            int nbrRequiredCellsVertically,
            int leftOverWidth,
            int leftOverHeight)
        {
            // Split the far most row and column if needed.
            if (leftOverWidth > 0)
            {
                this.nbrCellsGenerated += this.CanvasCells.NbrRows;

                int xFarRightColumn = x + nbrRequiredCellsHorizontally - 1;
                this.CanvasCells.InsertColumn(xFarRightColumn, leftOverWidth);
            }

            if (leftOverHeight > 0)
            {
                this.nbrCellsGenerated += this.CanvasCells.NbrColumns;

                int yFarBottomColumn = y + nbrRequiredCellsVertically - 1;
                this.CanvasCells.InsertRow(yFarBottomColumn, leftOverHeight);
            }

            for (int i = x + nbrRequiredCellsHorizontally - 1; i >= x; i--)
            {
                for (int j = y + nbrRequiredCellsVertically - 1; j >= y; j--)
                {
                    this.CanvasCells.SetItem(i, j, new CanvasCell(true));
                }
            }
        }

        /// <summary>
        /// Returns true if a rectangle with the given width and height can be placed
        /// in the cell with the given x and y, and its right and top neighbours.
        ///
        /// This method assumes that x,y is far away enough from the edges of the canvas
        /// that the rectangle could actually fit. So this method only looks at whether cells
        /// are occupied or not.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="requiredWidth"></param>
        /// <param name="requiredHeight"></param>
        /// <param name="nbrRequiredCellsHorizontally">
        /// Number of cells that the rectangle requires horizontally
        /// </param>
        /// <param name="nbrRequiredCellsVertically">
        /// Number of cells that the rectangle requires vertically
        /// </param>
        /// <param name="leftOverWidth">
        /// The amount of horizontal space left in the right most cells that could be used for the rectangle
        /// </param>
        /// <param name="leftOverHeight">
        /// The amount of vertical space left in the bottom most cells that could be used for the rectangle
        /// </param>
        /// <returns>
        /// Returns true if a rectangle with the given width and height can be placed
        /// in the cell with the given x and y, and its right and top neighbours.
        /// </returns>
        private bool IsAvailable(
            int x,
            int y,
            int requiredWidth,
            int requiredHeight,
            out int nbrRequiredCellsHorizontally,
            out int nbrRequiredCellsVertically,
            out int leftOverWidth,
            out int leftOverHeight)
        {
            nbrRequiredCellsHorizontally = 0;
            nbrRequiredCellsVertically = 0;
            leftOverWidth = 0;
            leftOverHeight = 0;

            int foundWidth = 0;
            int foundHeight = 0;
            int trialX = x;
            int trialY = y;

            // Check all cells that need to be unoccupied for there to be room for the rectangle.
            while (foundHeight < requiredHeight)
            {
                trialX = x;
                foundWidth = 0;

                while (foundWidth < requiredWidth)
                {
                    if (this.CanvasCells.Item(trialX, trialY).Occupied)
                    {
                        return false;
                    }

                    foundWidth += this.CanvasCells.ColumnWidth(trialX);
                    trialX++;
                }

                foundHeight += this.CanvasCells.RowHeight(trialY);
                trialY++;
            }

            // Visited all cells that we'll need to place the rectangle,
            // and none were occupied. So the space is available here.
            nbrRequiredCellsHorizontally = trialX - x;
            nbrRequiredCellsVertically = trialY - y;

            leftOverWidth = foundWidth - requiredWidth;
            leftOverHeight = foundHeight - requiredHeight;

            return true;
        }

        /// <summary>
        /// A Canvas Cell
        /// </summary>
        public struct CanvasCell
        {
            /// <summary>
            /// ?
            /// </summary>
            public bool Occupied;

            /// <summary>
            /// Initializes a new instance of the <see cref="CanvasCell"/> struct.
            /// </summary>
            /// <param name="occupied"></param>
            public CanvasCell(bool occupied)
            {
                this.Occupied = occupied;
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return this.Occupied ? "x" : ".";
            }
        }
    }
}

#pragma warning restore SA1614 // Element parameter documentation should have text
