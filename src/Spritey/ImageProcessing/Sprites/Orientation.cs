namespace Spritey.ImageProcessing.Sprites
{
    /// <summary>
    /// Determins how the sprite will be layed generated
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// Arrange all sprite images vertically
        /// </summary>
        VerticalOnly,

        /// <summary>
        /// Arrange all sprite images horizontally
        /// </summary>
        HorizontalOnly,

        /// <summary>
        /// Arrange all sprite images in the most "optimal" configuration.
        /// </summary>
        Optimal
    }
}
