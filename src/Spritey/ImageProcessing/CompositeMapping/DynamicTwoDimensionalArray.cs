namespace Spritey.ImageProcessing.CompositeMapping
{
    using System;
    using System.Text;

    /// <summary>
    /// Implements a two dimensional dynamic array with elements of type T.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    internal class DynamicTwoDimensionalArray<T>
    {
        // Describe the rows and columns
        private Dimension[] columns;
        private Dimension[] rows;

        private T[,] data;

        // Number of logical columns in the 2 dimensional array
        private int nbrColumns = 0;

        // Number of logical rows in the 2 dimensional array
        private int nbrRows = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTwoDimensionalArray{T}"/> class.
        /// Constructor
        /// </summary>
        internal DynamicTwoDimensionalArray()
        {
        }

        /// <summary>
        /// Gets Number of columns
        /// </summary>
        public int NbrColumns
        {
            get { return this.nbrColumns; }
        }

        /// <summary>
        /// Gets Number of rows
        /// </summary>
        public int NbrRows
        {
            get { return this.nbrRows; }
        }

        /// <summary>
        /// After you've constructed the array, you need to initialize it.
        ///
        /// This removes any content and creates the first cell - so the array
        /// will have height is 1 and width is 1.
        /// </summary>
        /// <param name="capacityX">
        /// The array will initially have capacity for at least this many columns.
        /// Must be greater than 0.
        /// Set to the expected maximum width of the array or greater.
        /// The array will resize itself if you make this too small, but resizing is expensive.
        /// </param>
        /// <param name="capacityY">
        /// The array will initially have capacity for at least this many rows.
        /// Must be greater than 0.
        /// Set to the expected maximum height of the array or greater.
        /// The array will resize itself if you make this too small, but resizing is expensive.
        /// </param>
        /// <param name="firstColumnWidth">
        /// Width of the first column.
        /// </param>
        /// <param name="firstRowHeight">
        /// Height of the first row.
        /// </param>
        /// <param name="firstCellValue">
        /// Value of the first cell.
        /// </param>
        public void Initialize(int capacityX, int capacityY, int firstColumnWidth, int firstRowHeight, T firstCellValue)
        {
            if (capacityX <= 0)
            {
                throw new Exception("capacityX cannot be 0 or smaller");
            }

            if (capacityY == 0)
            {
                throw new Exception("capacityY cannot be 0 or smaller");
            }

            if ((this.columns == null) || (this.columns.GetLength(0) < capacityX))
            {
                this.columns = new Dimension[capacityX];
            }

            if ((this.rows == null) || (this.rows.GetLength(0) < capacityY))
            {
                this.rows = new Dimension[capacityY];
            }

            if ((this.data == null) || (this.data.GetLength(0) < capacityX) || (this.data.GetLength(1) < capacityY))
            {
                this.data = new T[capacityX, capacityY];
            }

            this.nbrColumns = 1;
            this.nbrRows = 1;

            this.columns[0].Index = 0;
            this.columns[0].Size = firstColumnWidth;

            this.rows[0].Index = 0;
            this.rows[0].Size = firstRowHeight;

            this.data[0, 0] = firstCellValue;
        }

        /// <summary>
        /// Returns the item at the given location.
        /// </summary>
        /// <param name="x">the column index</param>
        /// <param name="y">the row index</param>
        /// <returns>the item value</returns>
        public T Item(int x, int y)
        {
            return this.data[this.columns[x].Index, this.rows[y].Index];
        }

        /// <summary>
        /// Sets an item to the given value
        /// </summary>
        /// <param name="x">the column index</param>
        /// <param name="y">the row index</param>
        /// <param name="value">the value</param>
        public void SetItem(int x, int y, T value)
        {
            this.data[this.columns[x].Index, this.rows[y].Index] = value;
        }

        /// <summary>
        /// Inserts a row at location y.
        /// If y equals 2, than all rows at y=3 and higher will now have y=4 and higher.
        /// The new row will have y=3.
        /// The contents of the row at y=2 will be copied to the row at y=3.
        ///
        /// If there is not enough capacity in the array for the additional row,
        /// than the internal data structure will be copied to a structure with twice the size
        /// (this copying is expensive).
        /// </summary>
        /// <param name="y">
        /// Identifies the row to be split.
        /// </param>
        /// <param name="heightNewRow">
        /// The height of the new row (the one at y=3 in the example).
        /// Must be smaller than the current height of the existing row.
        ///
        /// The old row will have height = (old height of old row) - (height of new row).
        /// </param>
        public void InsertRow(int y, int heightNewRow)
        {
            if (y >= this.nbrRows) {
                throw new Exception(string.Format("y is {0} but height is only {1}", y, this.nbrRows));
            }

            // If there are as many phyiscal rows as there are logical rows, we need to get more physical rows before the number
            // of logical rows can be increased.
            if (this.data.GetLength(1) == this.nbrRows)
            {
                this.IncreaseCapacity();
            }

            // Copy the cells with the given y to a new row after the last used row. The y of the new row equals _nbrRows.
            int physicalY = this.rows[y].Index;
            for (int x = 0; x < this.nbrColumns; x++)
            {
                this.data[x, this.nbrRows] = this.data[x, physicalY];
            }

            // Make room in the _rows array by shifting all items that come after the one indexed by y one position to the right.
            // If y is at the end of the array, there is no need to shift anything.
            if (y < (this.nbrRows - 1)) {
                Array.Copy(this.rows, y + 1, this.rows, y + 2, this.nbrRows - y - 1);
            }

            // Let the freed up element point at the newly copied row
            this.rows[y + 1].Index = this.nbrRows;

            // Set the heights of the old and new rows.
            int oldHeight = this.rows[y].Size;
            int newHeightExistingRow = oldHeight - heightNewRow;

            // Debug.Assert((heightNewRow > 0) && (newHeightExistingRow > 0));
            this.rows[y + 1].Size = heightNewRow;
            this.rows[y].Size = newHeightExistingRow;

            // The logical height of the array has increased by 1.
            this.nbrRows++;
        }

        /// <summary>
        /// Same as InsertRow, but than for columns.
        /// </summary>
        /// <param name="x">Identifies the column to be split</param>
        /// <param name="widthNewColumn">The width of the new column</param>
        public void InsertColumn(int x, int widthNewColumn)
        {
            if (x >= this.nbrColumns)
            {
                throw new Exception(string.Format("x is {0} but width is only {1}", x, this.nbrColumns));
            }

            // If there are as many phyiscal columns as there are logical columns, we need to get more physical columns before the number
            // of logical columns can be increased.
            if (this.data.GetLength(0) == this.nbrColumns)
            {
                this.IncreaseCapacity();
            }

            // Copy the cells with the given x to a new column after the last used column. The x of the new column equals _nbrColumns.
            int nbrPhysicalRows = this.data.GetLength(1);
            int physicalX = this.columns[x].Index;
            Array.Copy(this.data, physicalX * nbrPhysicalRows, this.data, this.nbrColumns * nbrPhysicalRows, this.nbrRows);

            // Make room in the _columns array by shifting all items that come after the one indexed by x one position to the right.
            // If x is at the end of the array, there is no need to shift anything.
            if (x < (this.nbrColumns - 1)) {
                Array.Copy(this.columns, x + 1, this.columns, x + 2, this.nbrColumns - x - 1);
            }

            // Let the freed up element point at the newly copied column
            this.columns[x + 1].Index = this.nbrColumns;

            // Set the widths of the old and new columns.
            int oldWidth = this.columns[x].Size;
            int newWidthExistingColumn = oldWidth - widthNewColumn;

            // Debug.Assert((widthNewColumn > 0) && (newWidthExistingColumn > 0));
            this.columns[x + 1].Size = widthNewColumn;
            this.columns[x].Size = newWidthExistingColumn;

            // The logical width of the array has increased by 1.
            this.nbrColumns++;
        }

        /// <summary>
        /// Returns the width of the column at the given location
        /// </summary>
        /// <param name="x">the location</param>
        /// <returns>the width of the column</returns>
        public int ColumnWidth(int x)
        {
            return this.columns[x].Size;
        }

        /// <summary>
        /// Returns the height of the row at the given location
        /// </summary>
        /// <param name="y">the location</param>
        /// <returns>the height of the row</returns>
        public int RowHeight(int y)
        {
            return this.rows[y].Size;
        }

        /// <summary>
        /// Represents the DynamicTowDimensionalArray as a string.
        /// </summary>
        /// <returns>two dimensional array as a string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();

            sb.Append(" X      ");
            for (int x = 0; x < this.nbrColumns + 1; x++)
            {
                sb.AppendFormat("   {0,2:G} ", x);
            }

            sb.AppendLine();

            sb.Append("Y       ");
            for (int x = 0; x < this.nbrColumns + 1; x++)
            {
                sb.AppendFormat(" ({0,3:G})", this.ColumnWidth(x));
            }

            sb.AppendLine();

            for (int y = 0; y < this.nbrRows + 1; y++)
            {
                sb.AppendFormat("{0,2:G} ({1,3:G}) ", y, this.RowHeight(y));

                for (int x = 0; x < this.nbrColumns + 1; x++)
                {
                    sb.AppendFormat("   {0}  ", this.Item(x, y));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Doubles the capacity of the internal data structures.
        ///
        /// Creates a new array with double the width and height of the old array.
        /// Copies the element of the old array to the new array.
        /// Then replaces the old array with the new array.
        /// </summary>
        private void IncreaseCapacity()
        {
            int oldCapacityX = this.data.GetLength(0);
            int oldCapacityY = this.data.GetLength(1);

            int newCapacityX = oldCapacityX * 2;
            int newCapacityY = oldCapacityY * 2;
            int nbrItemsToCopy = oldCapacityX * oldCapacityY;

            T[,] newData = new T[newCapacityX, newCapacityY];
            Array.Copy(this.data, newData, nbrItemsToCopy);

            this.data = newData;
        }

        /// <summary>
        /// Describes a row or column
        /// </summary>
        private struct Dimension
        {
            public short SizeValue;
            public short IndexValue;

            // The width of a column or the height of a row
            public int Size
            {
                get => (int)this.SizeValue;
                set => this.SizeValue = (short)value;
            }

            // When a row or column is split, the new row is created at the end of the physical array rather than in the middle.
            // That way, there is no need to copy lots of data. But it does mean you need indirection from the logical index
            // to the physical index.
            // This field provides the physical index.
            public int Index
            {
                get => (int)this.IndexValue;
                set => this.IndexValue = (short)value;
            }
        }
    }
}
