using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using static LogSystem.LogService;

namespace FileSerializer
{
	/// <summary>
	/// This class provides functions to serialize / deserialize an object in JSON
	///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public class JsonManager
	{
		private const string JSONEXTENTION = ".json";

		/// <summary>
		/// Default Constructor
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public JsonManager()
		{
		}

		public ReadWriteMonitoring ReadWriteMonitoring { get; } = new ReadWriteMonitoring();

		/// <summary>
		/// This function serializes an object in JSON and writes it to a file on disk
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The object to serialize.</param>
		/// <param name="FileName">The path to write the JSON serialized object.</param>
		public void SerializeAndWriteToFile(object sender, string FileName)
		{
			DataProgress.CurrentDataProgressStates = DataProgressStates.SAVING_TO_JSON;
			ReadWriteMonitoring.IsWritingDataToFile = true;
			FileNameAndLocation = "";

			try
			{
				// AppTrace.Log("Serializing ...", LogLevel.Info);
				string serializedData = JsonConvert.SerializeObject(sender, Formatting.Indented);
				// AppTrace.Log($"Serialized Json file with {serializedData.Length}.", LogLevel.Info);

				string resultpath = FileName + JSONEXTENTION;

				//   AppTrace.Log($"Writing to file {resultpath}...", LogLevel.Info);
				if(File.Exists(resultpath))
				{
					File.AppendAllText(resultpath, serializedData);
				}
				else
				{
					File.WriteAllText(resultpath, serializedData);
					FileNameAndLocation = resultpath;
				}
				//    AppTrace.Log($"Written Json to file {resultpath}.", LogLevel.Info);
			}
			catch(JsonException jex)
			{
				LogException(jex);
			}
			catch(IOException iex)
			{
				LogException(iex);
			}
			catch(Exception exception)
			{
				LogException(exception);
				throw new Exception("An error occurred while writing the JSON file!", exception);
			}

			ReadWriteMonitoring.IsWritingDataToFile = false;
		}


		/// <summary>
		/// This function serializes an object in JSON and writes it to a file on disk
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The object to serialize.</param>
		/// <param name="FileName">The path to write the JSON serialized object.</param>
		public void SerializeAndWriteToAnalysisFile(object sender, string FileName)
		{
			try
			{
				var serializedData = JsonConvert.SerializeObject(sender, Formatting.Indented);
				if(serializedData == null)
				{
					throw new JsonException(nameof(SerializeAndWriteToAnalysisFile));
				}
				if(File.Exists(FileName))
				{
					File.Delete(FileName);
				}
				File.WriteAllText(FileName, serializedData);
			}
			catch(Exception exception)
			{
				throw new JsonException(nameof(SerializeAndWriteToAnalysisFile), exception);
			}
		}

		public string SerializeAnalysisFile(object obj, string fileName)
		{
			if(obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}
			if(fileName == null)
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			try
			{
				var serializedData_ = JsonConvert.SerializeObject(obj, Formatting.Indented);
				if(serializedData_ == null)
				{
					throw new JsonException(nameof(SerializeAnalysisFile));
				}

				if(File.Exists(fileName))
				{
					File.Delete(fileName);
				}

				File.WriteAllText(fileName, serializedData_);
			}
			catch(Exception e)
			{
				LogException(e);
				throw new JsonException(nameof(SerializeAnalysisFile), e);
			}

			return fileName;
		}

		/// <summary>
		/// This function serializes an object in JSON and updates it to a file on disk
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="sender">The object to serialize.</param>
		/// <param name="FileName">The path to write the JSON serialized object.</param>
		public void SerializeAndUpdateExistingFile(object sender, string FileName)
		{
			ReadWriteMonitoring.IsWritingDataToFile = true;

			FileNameAndLocation = "";

			try
			{
				string serializedData = JsonConvert.SerializeObject(sender, Formatting.Indented);

				if(!File.Exists(FileName))
				{
					//The file should exist at this point
					throw new Exception();
				}
				else
				{
					File.WriteAllText(FileName, serializedData);
				}
			}
			catch(Exception exception)
			{
				throw new Exception("An error occurred while writing the JSON file!", exception);
			}

			ReadWriteMonitoring.IsWritingDataToFile = false;
		}

		/// <summary>
		/// This property gets and sets the file name and location on the drive
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public string FileNameAndLocation { get; set; } = string.Empty;

		/// <summary>
		/// This function deserializes a JSON file located on disk.  The JSON file contains
		/// a list of ablation data details objects
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="fileAndPath">The path and filename to deserialize.</param>
		/// <returns>List of ablation data details objects.</returns>
		public T DeserializeAblationData<T>(string fileAndPath) where T : class
		{
			T data = null;

			if(!ReadWriteMonitoring.IsWritingDataToFile)
			{
				if(!File.Exists(fileAndPath))
				{
					throw new FileNotFoundException();
				}
				else
				{
					using(StreamReader r = new StreamReader(fileAndPath))
					{
						string json = r.ReadToEnd();
						try
						{
							data = JsonConvert.DeserializeObject<T>(json);
						}
						catch(Exception e)
						{
							data = null;
						}
					}
				}
			}

			return data;
		}

		/// <summary>
		/// This function deserializes a JSON file located on disk.  The JSON file contains
		/// an engineering data details object
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="fileAndPath">The path and filename to deserialize.</param>
		/// <returns>An engineering data details objects.</returns>
		public EngineeringData DeserializeEngineeringData(string fileAndPath)
		{
			EngineeringData engineeringData = new EngineeringData();

			if(!File.Exists(fileAndPath))
			{
				throw new FileNotFoundException();
			}
			else
			{
				using(StreamReader r = new StreamReader(fileAndPath))
				{
					string json = r.ReadToEnd();
					engineeringData = JsonConvert.DeserializeObject<EngineeringData>(json);
				}
			}
			return engineeringData;
		}

		/// <summary>
		/// This function deserializes a JSON file located on disk.  The JSON file contains
		/// a list of ablation ECG data objects
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		/// <param name="fileAndPath">The path and filename to deserialize.</param>
		/// <returns>List of ablation ECG data objects.</returns>
		public List<AblationECGData> DeserializeAblationECGData(string fileAndPath)
		{
			List<AblationECGData> items = new List<AblationECGData>();

			if(!ReadWriteMonitoring.IsWritingDataToFile)
			{

				if(!File.Exists(fileAndPath))
				{
					throw new FileNotFoundException();
				}
				else
				{
					using(StreamReader r = new StreamReader(fileAndPath))
					{
						string json = r.ReadToEnd();
						items = JsonConvert.DeserializeObject<List<AblationECGData>>(json);
					}
				}
			}
			return items;
		}
	}

	/// <summary>
	/// Creates a static read write monitoring class
	///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public class ReadWriteMonitoring
	{
		/// <summary>
		/// Gets or sets whether we are writing data to file. 
		///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public bool IsWritingDataToFile { get; set; } = false;
	}
}