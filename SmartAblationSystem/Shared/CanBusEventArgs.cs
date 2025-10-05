// <copyright file="CanBusEventArgs.cs" company="company">
// Copyright (c) Cryterion Medical Inc. All rights reserved.
// </copyright>
// <author>Alex Smail</author>
// <date>01-17-2017</date>
// <summary> Handle CAN 1 and CAN 2 events</summary>

using System;

namespace Communication
{
    /// <summary>
    /// Handles CAN 1 and CAN 2 events
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CanBusEventArgs : EventArgs
    {
        private int falgs;
        private int cob;
        private uint id;
        private short length;
        private byte[] data;

        /// <summary>
        /// Gets or sets the event tag
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int Falgs
        {
            get
            {
                return falgs;
            }

            set
            {
                falgs = value;
            }
        }

        /// <summary>
        /// Gets or sets the COB
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int Cob
        {
            get
            {
                return cob;
            }

            set
            {
                cob = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public uint Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the length
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public short Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }

        /// <summary>
        /// Gets or sets the Data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public byte[] Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }
    }
}