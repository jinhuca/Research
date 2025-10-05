using System;
using System.Collections.Generic;
using System.IO;

namespace FileSerializer
{
    /// <summary>
    /// This class contains properties for Ablation Data details
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class EngineeringData
    {
        /// <summary>
        /// This property gets/sets EngineeringDataDetails list
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<EngineeringDataDetails> EngineeringDataDetails { get; set; }

        public const string engineeringReportFolder = "EngineeringReports\\";

        /// <summary>
        /// Default constructor that receives a list of ablation details and a list of ablation ECG details
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ablationDetails">A list of ablation details list.</param>
        /// <param name="ablationECGDetails">A list of ablation ECG details list.</param>
        public EngineeringData()
        {
            EngineeringDataDetails = new List<FileSerializer.EngineeringDataDetails>();
        }

        /// <summary>
        /// Clears Engineering data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ClearEngineeringData()
        {
            if (EngineeringDataDetails != null)
            {
                EngineeringDataDetails.Clear();
            }
        }

        /// <summary>
        /// Writes to JSON file
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">the sender</param>
        public void WriteToJson(object sender)
        {
            JsonManager jsonManager = null;
            EngineeringData engineeringData = null;
            string filePath = "";
            string filename = "";

            if (sender != null && sender is EngineeringData)
            {
                engineeringData = (EngineeringData)sender;

                if (engineeringData.EngineeringDataDetails != null &&
                    engineeringData.EngineeringDataDetails.Count > 0)
                {
                    filename = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" +
                               DateTime.Now.Hour + "h" + DateTime.Now.Minute + "m" + DateTime.Now.Second + "s" + DateTime.Now.Millisecond + "ms";

                    jsonManager = new JsonManager();

                    filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, engineeringReportFolder);
                    Directory.CreateDirectory(filePath); //create the directory if it does not exists, otherwise it does nothing
                    filePath = Path.Combine(filePath, filename);
                    jsonManager.SerializeAndWriteToFile(engineeringData, filePath);

                    //Clear the engineering data list
                    engineeringData.EngineeringDataDetails.Clear();
                }
            }
        }
    }
}