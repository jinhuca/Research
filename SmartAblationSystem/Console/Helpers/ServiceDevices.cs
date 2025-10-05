using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Helpers
{
    /// <summary>
    ///  Represents the devices that will be used on by service
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ServiceDevices
    {
        Catheter engineeringCatheter = new Catheter();

        private readonly int engineeringCatheterSignature = 128;

        /// <summary>
        /// Creates  the Service Devices  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ServiceDevices()
        {
            EngineeringCatheter.CatheterLot = 65535;
            EngineeringCatheter.SerialNumber = 255;


        }

        /// <summary>
        /// Gets or sets the engineering catheter
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <id>SF-SDS-0089</id>
        public Catheter EngineeringCatheter { get => engineeringCatheter; set => engineeringCatheter = value; }

        /// <summary>
        /// Gets or sets the engineering catheter signature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int EngineeringCatheterSignature => engineeringCatheterSignature;
    }
}
