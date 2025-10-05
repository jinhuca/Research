namespace Console
{
    /// <summary>
    /// Represents the catheter
    /// </summary>
    public class Catheter
    {
        /// <summary>
        /// 2-byte integer
        /// </summary>
        private int catheterID;

        //2 bytes integer
        private int serialNumber;

        private int catheterLot;

        //1 byte integer
        private int catheterExpirationDay;

        //1 byte integer
        private int catheterExpirationMonth;

        //2 bytes integer
        private int catheterExpirationYear;

        //1 byte
        private int catheterLastUseHour;

        //1 byte integer
        private int catheterLastUseDay;

        //1 byte integer
        private int catheterLastUseMonth;

        //2 bytes integer
        private int catheterLastUseYear;

        // 2 bytes integer
        private int numberOfInjections;

        /// <summary>
        /// Gets or sets the catheter ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterID
        {
            get
            {
                return catheterID;
            }

            set
            {
                catheterID = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter serial number
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int SerialNumber
        {
            get
            {
                return serialNumber;
            }

            set
            {
                serialNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter expiration day
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterExpirationDay
        {
            get
            {
                return catheterExpirationDay;
            }

            set
            {
                catheterExpirationDay = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter expiration month
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterExpirationMonth
        {
            get
            {
                return catheterExpirationMonth;
            }

            set
            {
                catheterExpirationMonth = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter expiration year
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterExpirationYear
        {
            get
            {
                return catheterExpirationYear;
            }

            set
            {
                catheterExpirationYear = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter last use month
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseMonth
        {
            get
            {
                return catheterLastUseMonth;
            }

            set
            {
                catheterLastUseMonth = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter last use year
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseYear
        {
            get
            {
                return catheterLastUseYear;
            }

            set
            {
                catheterLastUseYear = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter last use day
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseDay
        {
            get
            {
                return catheterLastUseDay;
            }

            set
            {
                catheterLastUseDay = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter number of injections
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int NumberOfInjections
        {
            get
            {
                return numberOfInjections;
            }

            set
            {
                numberOfInjections = value;
            }
        }

        /// <summary>
        /// Gets or sets the catheter last use hour
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLastUseHour
        {
            get
            {
                return catheterLastUseHour;
            }

            set
            {
                catheterLastUseHour = value;
            }
        }

        /// <summary>
        /// Gets the catheter lot
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int CatheterLot
        {
            get
            {
                return catheterLot;
            }

            set
            {
                catheterLot = value;
            }
        }
    }
}