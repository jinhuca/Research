using Module.Infrastructure.AppLog;
using Module.Infrastructure.Constants;
using Module.Report.Interfaces;
using PDFReportsGenerator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Module.Infrastructure;
using Module.TestProcess.Services;
using static Module.Infrastructure.Constants.Strings;
using static Module.Report.Constants.ReportConstants;

namespace Module.Report
{
	public class ReportPDF
	{
		public static void GeneratePdfReport(ITestReport testResult, string reportFileName)
		{
			if(testResult == null)
			{
				FieldServiceTrace.Log(NullTestResultMessage);
				return;
			}

			if(File.Exists(reportFileName))
			{
				try
				{
					File.Delete(reportFileName);
				}
				catch(IOException ioe)
				{
					FieldServiceTrace.LogException(ioe);
				}
			}

			var pdfTemplate_ = new PDFTemplate();
			var testResultPDF_ = new List<PDFElementsTable>();
			var testerFullName_ = $"{testResult.TesterReport.FirstName}{WhiteSpace}{testResult.TesterReport.LastName}";
			var overallTestResultImage_ = testResult.Passed == true ? PassedImage : FailedImage;
			var overallTestResult_ = testResult.Passed == true ? PassedMessage : FailedMessage;
			var imagePath_ = Path.Combine(Regex.Split(AppDomain.CurrentDomain.BaseDirectory, BinFolderName)[0], ImageFolderName);

			try
			{
				CreateSummaryInfo_();
				CreateManualTestsReport_();
				CreateParameterCheckReport_();
				CreatePerformanceTest_();
				CreateRetryRationaleReport_();
				CreateErrorMessageReport_();
				GenerateTestReport_();
			}
			catch(Exception e)
			{
				FieldServiceTrace.LogException(e);
			}
			
			void CreateSummaryInfo_()
			{
				var summaryElementValues_ = new string[5][];
				summaryElementValues_[0] = new[]
				{
					SiteText,
					$"{testResult.HospitalName ?? string.Empty}"
				};
				summaryElementValues_[1] = new[]
				{
					TestDateTimeText,
					$"{StartMsg}{testResult.StartDateTime}{DashWithSpace}{FinishMsg}{testResult.FinishDateTime}"
				};
				summaryElementValues_[2] = new[]
				{
					ConsoleSnText,
					$"{testResult.ConsoleSerialNumber ?? string.Empty}"
				};
				summaryElementValues_[3] = new[]
				{
					TesterFullNameText,
					$"{testerFullName_}"
				};
				summaryElementValues_[4] = new[]
				{
					FstVersion,
					$"{testResult.FstVersion ?? string.Empty}"
				};
				testResultPDF_.Add(new PDFElementsTable
				{
					ElementType = ElementTypeTableImage,
					ElementDispalyName = $"{OverallResultText}-{overallTestResult_}-{imagePath_}{overallTestResultImage_}",
					ElementValue = summaryElementValues_
				});
			}

			void CreateManualTestsReport_()
			{
				CreateManualTestSummary_();
				CreateVersionVerificationReport_();
				CreateInputTestReport_();
				CreateVisualTestReport_();
				CreateAudibleTestReport_();

				void CreateManualTestSummary_()
				{
					var _manualTestPassedImage = testResult.VersionReport?.Result?.Passed == true &&
																	testResult.InputReport?.Result?.Passed == true &&
																	testResult.VisualReport?.Result?.Passed == true &&
																	testResult.AudibleReport?.Result?.Passed == true
						? PassedImage
						: FailedImage;
					var manualTestValues_ = new string[2][];
					manualTestValues_[0] = new[] { WhiteSpace, WhiteSpace };
					manualTestValues_[1] = new[] { WhiteSpace, WhiteSpace };
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImage,
						ElementDispalyName = $"{ManualTestsText}-{imagePath_}{_manualTestPassedImage}",
						ElementValue = manualTestValues_
					});
				}

				void CreateVersionVerificationReport_()
				{
					if(testResult?.VersionReport?.Result == null)
					{
						return;
					}
					var versionTestResultImage_ = testResult.VersionReport.Result?.Passed == true ? PassedImage : FailedImage;
					var versionElementValues_ = new string[13][];
					versionElementValues_[0] = new[]
					{
						CMCUBootLoaderText,
						testResult.VersionReport?.Result?.CMCUBootVersion
					};
					versionElementValues_[1] = new[]
					{
						CMCUApplicationText,
						testResult.VersionReport?.Result?.CMCUVersion
					};
					versionElementValues_[2] = new[]
					{
						CPLDText,
						testResult.VersionReport?.Result?.CPLDVersion
					};
					versionElementValues_[3] = new[]
					{
						PMCUBootLoaderText,
						testResult.VersionReport?.Result?.PMCUBootVersion
					};
					versionElementValues_[4] = new[]
					{
						PMCUApplicationText,
						testResult.VersionReport?.Result?.PMCUVersion
					};
					versionElementValues_[5] = new[]
					{
						RMCUBootLoaderText,
						testResult.VersionReport?.Result?.RMCUBootVersion
					};
					versionElementValues_[6] = new[]
					{
						RMCUText,
						testResult.VersionReport?.Result?.RMCUVersion
					};
					versionElementValues_[7] = new[]
					{
						ICBBootLoaderText,
						testResult.VersionReport?.Result?.ICBBootVersion
					};
					versionElementValues_[8] = new[]
					{
						ICBApplicationText,
						testResult.VersionReport?.Result?.ICBVersion
					};
					versionElementValues_[9] = new[]
					{
						RCMCUBootLoaderText,
						testResult.VersionReport?.Result?.RCMCUBootVersion
					};
					versionElementValues_[10] = new[]
					{
						RCMCUText,
						testResult.VersionReport?.Result?.RCMCUVersion
					};
					versionElementValues_[11] = new[]
					{
						GUIText,
						testResult.VersionReport?.Result?.GUIVersion
					};
					versionElementValues_[12] = new[]
					{
						DBText,
						testResult.VersionReport?.Result?.DBVersion
					};
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImage,
						ElementDispalyName = $"{VersionVerificationText}-{imagePath_}{versionTestResultImage_}",
						ElementValue = versionElementValues_
					});
				}

				void CreateInputTestReport_()
				{
					if(testResult?.InputReport?.Result == null)
					{
						return;
					}
					var inputTestResultImage_ = testResult.InputReport.Result.Passed == true ? PassedImage : FailedImage;
					var inputElementValues_ = new string[5][];
					inputElementValues_[0] = new[]
					{
						TestText,
						ExpectedText,
						ActualText,
						ResultText
					};

					if(testResult.InputReport.Result.StartPushButtonStatus.HasValue)
					{
						var result_ = testResult.InputReport.Result.StartPushButtonStatus;
						inputElementValues_[1] = new[]
						{
							StartPushButtonText,
							OnText,
							result_.Value ? OnText : OffText,
							result_.Value ? PassedMessage : FailedMessage
						};
					}
					else
					{
						inputElementValues_[1] = new[]
						{
							WhiteSpace, WhiteSpace, WhiteSpace, WhiteSpace
						};
					}

					if(testResult.InputReport.Result.StopPushButtonStatus.HasValue)
					{
						var result_ = testResult.InputReport.Result.StopPushButtonStatus;
						inputElementValues_[2] = new[]
						{
							StopPushButtonText,
							OnText,
							result_.Value ? OnText : OffText,
							result_.Value ? PassedMessage : FailedMessage
						};
					}
					else
					{
						inputElementValues_[2] = new[]
						{
							WhiteSpace, WhiteSpace, WhiteSpace, WhiteSpace
						};
					}

					if(testResult.InputReport.Result.StartFootSwitch.HasValue)
					{
						var result_ = testResult.InputReport.Result.StartFootSwitch;
						inputElementValues_[3] = new[]
						{
							StartFootSwitchText,
							OnText,
							result_.Value ? OnText : OffText,
							result_.Value ? PassedMessage : FailedMessage
						};
					}
					else
					{
						inputElementValues_[3] = new[]
						{
							WhiteSpace, WhiteSpace, WhiteSpace, WhiteSpace
						};
					}

					if(testResult.InputReport.Result.StopFootSwitch.HasValue)
					{
						var result_ = testResult.InputReport.Result.StopFootSwitch;
						inputElementValues_[4] = new[]
						{
							StopFootSwitchText,
							OnText,
							result_.Value ? OnText : OffText,
							result_.Value ? PassedMessage : FailedMessage
						};
					}
					else
					{
						inputElementValues_[4] = new[]
						{
							WhiteSpace, WhiteSpace, WhiteSpace, WhiteSpace
						};
					}

					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImageResult,
						ElementDispalyName = $"{InputTestText}-{imagePath_}{inputTestResultImage_}",
						ElementValue = inputElementValues_
					});
				}

				void CreateVisualTestReport_()
				{
					if(testResult?.VisualReport?.Result == null)
					{
						return;
					}
					var visualTestResultImage_ = testResult.VisualReport.Result.Passed == true ? PassedImage : FailedImage;
					var visualElementValues_ = new string[3][];

					visualElementValues_[0] = new[]
					{
						TestText,
						ExpectedText,
						WhiteSpace,
						ResultText
					};

					if(testResult.VisualReport.Result.LEDsStatus.HasValue)
					{
						var result_ = testResult.VisualReport.Result.LEDsStatus;
						visualElementValues_[1] = new[]
						{
							ConsoleLEDsText,
							OnOffText,
							WhiteSpace,
							result_.Value ? PassedMessage : FailedMessage
						};
					}
					else
					{
						visualElementValues_[1] = new[]
						{
							WhiteSpace, WhiteSpace, WhiteSpace, WhiteSpace
						};
					}

					if(testResult.VisualReport.Result.ScreenStatus.HasValue)
					{
						var result_ = testResult.VisualReport.Result.ScreenStatus;
						visualElementValues_[2] = new[]
						{
							DisplayTestText,
							DisplayMessageText,
							WhiteSpace,
							result_.Value ? PassedMessage : FailedMessage
						};
					}
					else
					{
						visualElementValues_[2] = new[]
						{
							WhiteSpace, WhiteSpace, WhiteSpace, WhiteSpace
						};
					}

					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImageResult,
						ElementDispalyName = $"{VisualTestText}-{imagePath_}{visualTestResultImage_}",
						ElementValue = visualElementValues_
					});
				}

				void CreateAudibleTestReport_()
				{
					if(testResult.AudibleReport?.Result == null)
					{
						return;
					}
					var audibleTestResultImage_ = testResult.AudibleReport.Result.Passed == true ? PassedImage : FailedImage;
					var audibleTestResult_ = testResult.AudibleReport.Result.Passed == true ? PassedMessage : FailedMessage;
					var audibleElementValues_ = new string[2][];
					audibleElementValues_[0] = new[]
					{
						TestText,
						ExpectedText,
						WhiteSpace,
						ResultText
					};
					audibleElementValues_[1] = new[]
					{
						SpeakerText,
						AudibleMessage,
						WhiteSpace,
						audibleTestResult_
					};
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImageResult,
						ElementDispalyName = $"{AudibleTestText}-{imagePath_}{audibleTestResultImage_}",
						ElementValue = audibleElementValues_
					});
				}
			}

			void CreateParameterCheckReport_()
			{
				if(testResult.IdleStateReport?.Result == null)
				{
					return;
				}
				CreateNewPage_();
				CreateParameterCheckSummary_();
				CreateIdleStateReport_();
				CreateReadyStateReport_();

				void CreateParameterCheckSummary_()
				{
					var parameterCheckPassedImage_ = testResult.IdleStateReport?.Result?.Passed == true && testResult.ReadyStateReport?.Result?.Passed == true
						? PassedImage
						: FailedImage;
					var parameterCheckValues_ = new string[2][];
					parameterCheckValues_[0] = new[] { WhiteSpace, WhiteSpace };
					parameterCheckValues_[1] = new[] { WhiteSpace, WhiteSpace };
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImage,
						ElementDispalyName = $"{ParameterCheckText}-{imagePath_}{parameterCheckPassedImage_}",
						ElementValue = parameterCheckValues_
					});
				}

				void CreateIdleStateReport_()
				{
					if(testResult?.IdleStateReport?.Result == null)
					{
						return;
					}
					var idleStateTestResultImage_ = testResult.IdleStateReport.Result.Passed == true ? PassedImage : FailedImage;
					var idleStatElementValues_ = new string[6][];
					idleStatElementValues_[0] = new[]
					{
						TestText,
						ExpectedText,
						ActualText,
						ResultText
					};

					idleStatElementValues_[1] = new[]
					{
						AvgFM1Text,
						FM1ThresholdText,
						testResult.IdleStateReport.Result.FM1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						testResult.IdleStateReport.Result.FM1Avg.Passed == true ? PassedMessage : FailedMessage
					};

					idleStatElementValues_[2] = new[]
					{
						AvgPT1Text,
						PT1ThresholdText,
						testResult.IdleStateReport.Result.PT1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						PT1Result
					};

					idleStatElementValues_[3] = new[]
					{
						AvgLC1Text,
						PT1Result,
						testResult.IdleStateReport.Result.LC1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						LC1ThresholdText
					};

					idleStatElementValues_[4] = new[]
					{
						AvgPT3Text,
						PT3Threshold,
						testResult.IdleStateReport.Result.PT3Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						PT3Result
					};

					idleStatElementValues_[5] = new[]
					{
						AvgTS1Text,
						AvgTS1ThresholdText,
						testResult.IdleStateReport.Result.TS1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						testResult.IdleStateReport.Result.TS1Avg.Passed == true ? PassedMessage : FailedMessage
					};

					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImageResult,
						ElementDispalyName = $"{IdleCheckText}-{imagePath_}{idleStateTestResultImage_}",
						ElementValue = idleStatElementValues_
					});
				}

				void CreateReadyStateReport_()
				{
					if(testResult.ReadyStateReport?.Result == null)
					{
						return;
					}
					var readyStateTestResultImage_ = testResult.ReadyStateReport.Result.Passed == true ? PassedImage : FailedImage;
					var readyStatElementValues_ = new string[6][];
					readyStatElementValues_[0] = new[]
					{
						TestText,
						ExpectedText,
						ActualText,
						ResultText
					};
					readyStatElementValues_[1] = new[]
					{
						AvgFM1Text,
						FM1ThresholdText,
						testResult.ReadyStateReport?.Result?.FM1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						testResult.ReadyStateReport?.Result?.FM1Avg.Passed == true ? PassedMessage : FailedMessage
					};
					readyStatElementValues_[2] = new[]
					{
						AvgPT1Text,
						PT1ThresholdText,
						testResult.ReadyStateReport?.Result?.PT1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						PT1Result
					};
					readyStatElementValues_[3] = new[]
					{
						AvgLC1Text,
						LC1ThresholdText,
						testResult.ReadyStateReport?.Result?.LC1Avg.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						NAText
					};
					readyStatElementValues_[4] = new[]
					{
						MaxOBPText,
						LessEqualText + Math.Round(BalloonPressureThreshold - (testResult.IdleStateReport.Result.PT3Avg.Value + OBPFactor), RoundOneDigit),
						testResult.ReadyStateReport?.Result?.OBPMax.Value.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
						testResult.ReadyStateReport?.Result?.OBPMax.Passed == true ? PassedMessage : FailedMessage
					};
					readyStatElementValues_[5] = new[]
					{
						AvgIBPText,
						LessEqualText + Math.Round(BalloonPressureThreshold - (testResult.IdleStateReport.Result.PT3Avg.Value + OBPFactor), RoundOneDigit),
						testResult.ReadyStateReport?.Result?.IBPAvg.Value.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
						testResult.ReadyStateReport?.Result?.IBPAvg.Passed == true? PassedMessage : FailedMessage
					};
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImageResult,
						ElementDispalyName = $"{ReadyCheckText}-{imagePath_}{readyStateTestResultImage_}",
						ElementValue = readyStatElementValues_
					});
				}
			}

			void CreateNewPage_()
			{
				testResultPDF_.Add(new PDFElementsTable { ElementType = ElementTypeNewPage });
			}

			void CreatePerformanceTest_()
			{
				if(testResult.AblationReport?.Result == null)
				{
					return;
				}
				CreateNewPage_();
				CreatePerformanceTestSummary_();
				CreateAblationTestsReport_();

				void CreatePerformanceTestSummary_()
				{
					var isPerformanceTestPassedImage_ = testResult.AblationReport?.Result?.Passed == true ? PassedImage : FailedImage;
					var performanceTestValues_ = new string[2][];
					performanceTestValues_[0] = new[] { WhiteSpace, WhiteSpace };
					performanceTestValues_[1] = new[] { WhiteSpace, WhiteSpace };
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableImage,
						ElementDispalyName = $"{PerformanceTestText}-{imagePath_}{isPerformanceTestPassedImage_}",
						ElementValue = performanceTestValues_
					});
				}

				void CreateAblationTestsReport_()
				{
					var inflationpassed = false;
					var ablationpassed = false;
					var thawingpassed = false;

					CreateAblationSummary_();
					CreateInflationTestReport_();
					CreateAblationTestReport_();
					CreateThawingTestReport_();

					void CreateAblationSummary_()
					{
						var isAblationTestsPassedImage_ = testResult.AblationReport?.Result?.Passed == true ? PassedImage : FailedImage;
						var ablationTestValues_ = new string[2][];
						ablationTestValues_[0] = new[] { WhiteSpace, WhiteSpace };
						ablationTestValues_[1] = new[] { WhiteSpace, WhiteSpace };
						testResultPDF_.Add(new PDFElementsTable
						{
							ElementType = ElementTypeTableImage,
							ElementDispalyName = $"{AblationTestsText}-{imagePath_}{isAblationTestsPassedImage_}",
							ElementValue = ablationTestValues_
						});
					}
					void CreateInflationTestReport_()
					{
						var inflationData_ = testResult.AblationReport?.Result?.AblationResult?[AblationTestState.INFLATION];
						if(inflationData_ == null)
						{
							return;
						}
						var inflationCount_ = inflationData_[TestParameter.Inflation_Speed].Count;
						if(inflationCount_ == 0)
						{
							return;
						}

						var _inflationResults = GetInflationTestResults();
						CreateInflationTitle_();
						CreateInflationSpeedTestReport_();
						CreateInflationIBPTestReport_();
						CreateNewPage_();
						CreateInflationOBPTestReport_();
						CreateInflationPT2TestReport_();
						CreateInflationFM1TestReport_();

						IDictionary<TestParameter, bool?> GetInflationTestResults()
						{
							var resultdict = new Dictionary<TestParameter, bool?>();
							foreach (KeyValuePair<TestParameter, List<(double, bool?, double?)>> entry in inflationData_){
								bool? result = entry.Value.All(z => z.Item2 == null) ? null : (bool?)!entry.Value.Select(x => x.Item2).Any(y => y.Value == false);
								resultdict.Add(entry.Key, result);
							}
							inflationpassed = !resultdict.Any(x => x.Value == false);
							return resultdict;
						}

						void CreateInflationTitle_()
						{
							var inflationTitleValues_ = new string[2][];
							var isinflationpassedimage = inflationpassed ? PassedImage : FailedImage;
							var isinflationpassedmessage = inflationpassed ? PassedMessage : FailedMessage;
							inflationTitleValues_[0] = new[] { WhiteSpace, WhiteSpace };
							inflationTitleValues_[1] = new[] { WhiteSpace, WhiteSpace };
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTableImage,
								ElementDispalyName = $"{InflationTestTitleText}-{imagePath_}{isinflationpassedimage}",
								ElementValue = inflationTitleValues_
							});
						}

						void CreateInflationSpeedTestReport_()
						{
							if(inflationData_[TestParameter.Inflation_Speed] == null || inflationData_[TestParameter.Inflation_Speed].Count == 0)
							{
								return;
							}
							var inflationSpeedElementValues_ = new string[inflationCount_ + 1][];
							inflationSpeedElementValues_[0] = new[] { InflationSpeedText, ExpectedText, ActualText, ResultText };
							for(int indexInflation_ = 0; indexInflation_ < inflationCount_; indexInflation_++)
							{
								if(inflationData_[TestParameter.Inflation_Speed]?.ElementAt(indexInflation_) == null)
								{
									continue;
								}

                var expected_ = inflationData_[TestParameter.Inflation_Speed]?.ElementAt(indexInflation_).Item3;
                var isFastInflation_ = expected_ < ServiceConstants.SlowInflationDelta;

								var expectedDisplayed_ = string.Empty;
								if(expected_ != null)
                {
	                expectedDisplayed_ = Math.Round(expected_.Value, RoundTwoDigits).ToString(TwoDecimalPlace, CultureInfo.CurrentCulture);
                }
								else
								{
									continue;
								}

                inflationSpeedElementValues_[indexInflation_ + 1] = new[]
								{
									$"{InflationIndexText}{indexInflation_ + 1}",
									isFastInflation_ ? InflationSpeedExpectedText2 : $"{expectedDisplayed_}{InflationSpeedExpectedText1}",
									inflationData_[TestParameter.Inflation_Speed] == null
										? string.Empty
										: inflationData_[TestParameter.Inflation_Speed]?.ElementAt(indexInflation_).Item1.ToString(ThreeDecimalPlace, CultureInfo.CurrentCulture),
									inflationData_[TestParameter.Inflation_Speed] == null
										? string.Empty
										: inflationData_[TestParameter.Inflation_Speed]?.ElementAt(indexInflation_).Item2 == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = _inflationResults[TestParameter.Inflation_Speed];
              var displaytitle = res != null ? (res.Value ? $"{InflationSpeedText}-{imagePath_}{PassedImage}" : $"{InflationSpeedText}-{imagePath_}{FailedImage}") : $"{InflationSpeedText}";
              testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = inflationSpeedElementValues_
							});
						}

						void CreateInflationIBPTestReport_()
						{
							if(inflationData_[TestParameter.Inflation_IBP] == null || inflationData_[TestParameter.Inflation_IBP].Count == 0)
							{
								return;
							}
							var inflationIBPElementValues_ = new string[inflationCount_ + 1][];
							inflationIBPElementValues_[0] = new[] { InflationIBPText, ExpectedText, ActualText, ResultText };
							for(int indexInflation_ = 0; indexInflation_ < inflationCount_; indexInflation_++)
							{
								if(inflationData_[TestParameter.Inflation_IBP]?.ElementAt(indexInflation_) == null)
								{
									continue;
								}

                var parameter = inflationData_[TestParameter.Inflation_IBP]?.ElementAt(indexInflation_).Item3 ?? -1d; 
                var inflationIBPThreshold = parameter > 0d 
                    ? InflationIBPDASBalloonThreshold
                    : InflationIBPThreshold;

                inflationIBPElementValues_[indexInflation_ + 1] = new[]
								{
									$"{InflationIndexText}{indexInflation_ + 1}",
                  inflationIBPThreshold,
									inflationData_[TestParameter.Inflation_IBP].ElementAt(indexInflation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									inflationData_[TestParameter.Inflation_IBP] == null
										? string.Empty
										: inflationData_[TestParameter.Inflation_IBP]?.ElementAt(indexInflation_).Item2 == true? PassedMessage:FailedMessage
								};
							}
							bool? res = _inflationResults[TestParameter.Inflation_IBP];
							var displaytitle = res != null ? (res.Value ? $"{InflationIBPText}-{imagePath_}{PassedImage}" : $"{InflationIBPText}-{imagePath_}{FailedImage}") : $"{InflationIBPText}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = inflationIBPElementValues_
							});
						}

						void CreateInflationOBPTestReport_()
						{
							if(inflationData_[TestParameter.Inflation_OBP]?.ElementAt(0) == null)
							{
								return;
							}
							var inflationOBPElementValues_ = new string[2][];
							inflationOBPElementValues_[0] = new[] { InflationOBPText, ExpectedText, ActualText, ResultText };
							inflationOBPElementValues_[1] = new[]
							{
								InflationOBPTestText,
								LessEqualText + inflationData_[TestParameter.Inflation_OBP]?.ElementAt(0).Item3?.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
								inflationData_[TestParameter.Inflation_OBP]?.ElementAt(0).Item1.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
								inflationData_[TestParameter.Inflation_OBP]?.ElementAt(0).Item2 == true ? PassedMessage : FailedMessage
							};
							bool? res = _inflationResults[TestParameter.Inflation_OBP];
							var displaytitle = res != null ? (res.Value ? $"{InflationOBPText}-{imagePath_}{PassedImage}" : $"{InflationOBPText}-{imagePath_}{FailedImage}") : $"{InflationOBPText}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = inflationOBPElementValues_
							});
						}

						void CreateInflationPT2TestReport_()
						{
							if(inflationData_[TestParameter.Inflation_PT2] == null || inflationData_[TestParameter.Inflation_PT2].Count == 0)
							{
								return;
							}
							var inflationPT2ElementValues_ = new string[inflationCount_ + 1][];
							inflationPT2ElementValues_[0] = new[] { InflationPT2Text, ExpectedText, ActualText, ResultText };
							for(int indexInflation_ = 0; indexInflation_ < inflationCount_; indexInflation_++)
							{
								if(inflationData_[TestParameter.Inflation_PT2]?.ElementAt(indexInflation_) == null)
								{
									continue;
								}
								inflationPT2ElementValues_[indexInflation_ + 1] = new[]
								{
									$"{InflationIndexText}{indexInflation_ + 1}",
									InflationPT2ExpectedText,
									inflationData_[TestParameter.Inflation_PT2]?.ElementAt(indexInflation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									inflationData_[TestParameter.Inflation_PT2]?.ElementAt(indexInflation_).Item2 == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = _inflationResults[TestParameter.Inflation_PT2];
							var displaytitle = res != null ? (res.Value ? $"{InflationPT2Text}-{imagePath_}{PassedImage}" : $"{InflationPT2Text}-{imagePath_}{FailedImage}") : $"{InflationPT2Text}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = inflationPT2ElementValues_
							});
						}

						void CreateInflationFM1TestReport_()
						{
							if(inflationData_[TestParameter.Inflation_FM1] == null || inflationData_[TestParameter.Inflation_FM1].Count == 0)
							{
								return;
							}
							var inflationFM1ElementValues_ = new string[inflationCount_ + 1][];
							inflationFM1ElementValues_[0] = new[] { InflationFM1Text, ExpectedText, ActualText, ResultText };
							for(int indexInflation_ = 0; indexInflation_ < inflationCount_; indexInflation_++)
							{
								if(inflationData_[TestParameter.Inflation_FM1]?.ElementAt(indexInflation_) == null)
								{
									continue;
								}
								inflationFM1ElementValues_[indexInflation_ + 1] = new[]
								{
									$"{InflationIndexText}{indexInflation_ + 1}",
									InflationFM1Threshold,
									inflationData_[TestParameter.Inflation_FM1]?.ElementAt(indexInflation_).Item1.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
									inflationData_[TestParameter.Inflation_FM1]?.ElementAt(indexInflation_).Item2  == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = _inflationResults[TestParameter.Inflation_FM1];
							var displaytitle = res != null ? (res.Value ? $"{InflationFM1Text}-{imagePath_}{PassedImage}" : $"{InflationFM1Text}-{imagePath_}{FailedImage}") : $"{InflationFM1Text}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = inflationFM1ElementValues_
							});
						}
					}

					void CreateAblationTestReport_()
					{
						var ablationData_ = testResult.AblationReport?.Result?.AblationResult?[AblationTestState.ABLATION];
						if(ablationData_ == null)
						{
							return;
						}
						var ablationCount_ = ablationData_[TestParameter.Ablation_FM1].Count;
						if(ablationCount_ == 0)
						{
							return;
						}

						var ablationResults = GetAblationTestResults();
						CreateNewPage_();
						CreateAblationTitle_();
						CreateSmoothCheckReport_();
						CreateAblationFlowMeterCheck_();
						CreateAblationTimeInTransitionReport_();
						CreateAblationFM1Report_();
						CreateAblationPT2Report_();

						CreateNewPage_();
						CreateAblationIBPReport_();
						CreateAblationOBPReport_();
						CreateAblationLowestTC1Report_();

						CreateNewPage_();
						CreateAblationTimeToMinus50Report();
						CreateAblationPWM1Report_();
						CreateAblationPWM2Report_();

						CreateNewPage_();
						CreateAblationPT3Report_();
						CreateAblationPT4Report_();
						CreateAblationPT5Report_();

						CreateNewPage_();
						CreateAblationTS1Report_();

						IDictionary<TestParameter, bool?> GetAblationTestResults()
						{
							var resultdict = new Dictionary<TestParameter, bool?>();
							foreach (KeyValuePair<TestParameter, List<(double, bool?, double?)>> entry in ablationData_)
							{
								bool? result = entry.Value.All(z => z.Item2 == null) ? null : (bool?)!entry.Value.Select(x => x.Item2).Any(y => y.Value == false);
								resultdict.Add(entry.Key, result);
							}
							ablationpassed = !resultdict.Any(x => x.Value == false);
							return resultdict;
						}

						void CreateAblationTitle_()
						{
							var ablationTitleValues_ = new string[2][];
							ablationTitleValues_[0] = new[] { WhiteSpace, WhiteSpace };
							ablationTitleValues_[1] = new[] { WhiteSpace, WhiteSpace };
							var isablationpassedimage = ablationpassed ? PassedImage : FailedImage;
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTableImage,
								ElementDispalyName = $"{AblationTestTitleText}-{imagePath_}{isablationpassedimage}",
								ElementValue = ablationTitleValues_
							});
						}

						void CreateSmoothCheckReport_()
						{
							if(testResult.AblationReport.Result == null)
							{
								return;
							}
							var smoothnesspassedimage = testResult.AblationReport.Result.Smoothness ? PassedImage : FailedImage;
							var smoothnessCheckElementValues_ = new string[2][];
							smoothnessCheckElementValues_[0] = new[] { SmoothnessCheckText, WhiteSpace, WhiteSpace, ResultText };
							smoothnessCheckElementValues_[1] = new[]
							{
								SmoothnessCheckTestText,
								WhiteSpace,
								WhiteSpace,
								testResult.AblationReport.Result.Smoothness? PassedMessage : FailedMessage
							};
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTableImageResult,
								ElementDispalyName = $"{SmoothnessCheckText}-{imagePath_}{smoothnesspassedimage}",
								ElementValue = smoothnessCheckElementValues_
							});
						}

						void CreateAblationFlowMeterCheck_()
						{
							var flowMeterCheckElementValues_ = new string[2][];
							flowMeterCheckElementValues_[0] = new[] { FlowMeterCheckText, ExpectedText, ActualText, ResultText };

							var actualValue = ablationData_[TestParameter.Ablation_FlowMeterCheck]?.ElementAt(0).Value;
							var passFail = ablationData_[TestParameter.Ablation_FlowMeterCheck]?.ElementAt(0).Passed;
							var flowmeterpassedimage = (passFail == null || true) ? PassedImage : FailedImage;
							flowMeterCheckElementValues_[1] = new[]
							{
								FlowMeterCheckTestText,
								LessEqualText + @"2.0%",

								double.IsNaN(actualValue??Double.NaN)
									? NotAvailableMessage
									: $"{actualValue}%",

								passFail.HasValue
									? passFail == true
										? PassedMessage
										: FlowMeterCheckSkippedText
									: FlowMeterCheckSkippedText
							};

							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTableImageResult,
								ElementDispalyName = $"{FlowMeterCheckText}-{imagePath_}{flowmeterpassedimage}",
								ElementValue = flowMeterCheckElementValues_
							});

						}

						void CreateAblationTimeInTransitionReport_()
						{
							if(ablationData_[TestParameter.Transition_Time] == null || ablationData_[TestParameter.Transition_Time].Count == 0)
							{
								return;
							}
							var ablationTimeInTransitionValues_ = new string[ablationCount_ + 1][];
							ablationTimeInTransitionValues_[0] = new[] { AblationTimeInTransitionText, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Transition_Time]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationTimeInTransitionValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									TransitionTimeRangeText,
									ablationData_[TestParameter.Transition_Time]?.ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									ablationData_[TestParameter.Transition_Time]?.ElementAt(indexAblation_).Item2 == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Transition_Time];
							var displaytitle = res != null ? (res.Value ? $"{AblationTimeInTransitionText}-{imagePath_}{PassedImage}" : $"{AblationTimeInTransitionText}-{imagePath_}{FailedImage}") : $"{AblationTimeInTransitionText}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationTimeInTransitionValues_
							});
						}

						void CreateAblationFM1Report_()
						{
							if(ablationData_[TestParameter.Ablation_FM1] == null || ablationData_[TestParameter.Ablation_FM1].Count == 0)
							{
								return;
							}
							var ablationFM1ElementValues_ = new string[ablationCount_ + 1][];
							ablationFM1ElementValues_[0] = new[] { AblationFM1Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_FM1]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}

                var parameter = ablationData_[TestParameter.Ablation_FM1].ElementAt(indexAblation_).Item3 ?? -1d;
                var ablationFM1ThresholdText = parameter > 0 
	                ? AblationFM1DASBalloonThresholdText : AblationFM1ThresholdText;

                ablationFM1ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
                  ablationFM1ThresholdText,
									ablationData_[TestParameter.Ablation_FM1].ElementAt(indexAblation_).Item1.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
									ablationData_[TestParameter.Ablation_FM1].ElementAt(indexAblation_).Item2  == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Ablation_FM1];
							var displaytitle = res != null ? (res.Value ? $"{AblationFM1Text}-{imagePath_}{PassedImage}" : $"{AblationFM1Text}-{imagePath_}{FailedImage}") : $"{AblationFM1Text}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationFM1ElementValues_
							});
						}

						void CreateAblationPT2Report_()
						{
							if(ablationData_[TestParameter.Ablation_PT2] == null || ablationData_[TestParameter.Ablation_PT2].Count == 0)
							{
								return;
							}
							var ablationPT2ElementValues_ = new string[ablationCount_ + 1][];
							ablationPT2ElementValues_[0] = new[] { AblationPT2Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_PT2]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationPT2ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									AblationPT2ThresholdText,
									ablationData_[TestParameter.Ablation_PT2].ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									ablationData_[TestParameter.Ablation_PT2].ElementAt(indexAblation_).Item2  == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Ablation_PT2];
							var displaytitle = res != null ? (res.Value ? $"{AblationPT2Text}-{imagePath_}{PassedImage}" : $"{AblationPT2Text}-{imagePath_}{FailedImage}") : $"{AblationPT2Text}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationPT2ElementValues_
							});
						}

						void CreateAblationIBPReport_()
						{
							if(ablationData_[TestParameter.Ablation_IBP] == null || ablationData_[TestParameter.Ablation_IBP].Count == 0)
							{
								return;
							}
							var ablationIBPElementValues_ = new string[ablationCount_ + 1][];
							ablationIBPElementValues_[0] = new[] { AblationIBPText, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_IBP]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								var validatedResult_ = ablationData_[TestParameter.Ablation_IBP].ElementAt(indexAblation_).Item2;

								var parameter = ablationData_[TestParameter.Ablation_IBP]?.ElementAt(indexAblation_).Item3 ?? -1d;
                var ablationIBPThreshold = parameter > 0d
                  ? AblationIBPDASBalloonThresholdText
                  : AblationIBPThresholdText;

                ablationIBPElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									validatedResult_ == null ? NAText : ablationIBPThreshold,
                  ablationData_[TestParameter.Ablation_IBP]?.ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									validatedResult_  == null ? NAText : validatedResult_ == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Ablation_IBP];
							var displaytitle = res != null ? (res.Value ? $"{AblationIBPText}-{imagePath_}{PassedImage}" : $"{AblationIBPText}-{imagePath_}{FailedImage}") : $"{AblationIBPText}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationIBPElementValues_
							});
						}

						void CreateAblationOBPReport_()
						{
							if(ablationData_[TestParameter.Ablation_OBP] == null || ablationData_[TestParameter.Ablation_OBP].Count == 0)
							{
								return;
							}
							var ablationOBPElementValues_ = new string[ablationCount_ + 1][];
							ablationOBPElementValues_[0] = new[] { AblationOBPText, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_OBP]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								string expected_ = ablationData_[TestParameter.Ablation_OBP].ElementAt(indexAblation_).Item3.HasValue
									? ablationData_[TestParameter.Ablation_OBP]?.ElementAt(indexAblation_).Item3?.ToString(OneDecimalPlace, CultureInfo.InstalledUICulture)
									: NAText;

								if(expected_ != NAText)
								{
									expected_ = "<= " + expected_;
								}

								ablationOBPElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									expected_,
									ablationData_[TestParameter.Ablation_OBP]?.ElementAt(indexAblation_).Item1.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
									ablationData_[TestParameter.Ablation_OBP]?.ElementAt(indexAblation_).Item2 == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Ablation_OBP];
							var displaytitle = res != null ? (res.Value ? $"{AblationOBPText}-{imagePath_}{PassedImage}" : $"{AblationOBPText}-{imagePath_}{FailedImage}") : $"{AblationOBPText}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationOBPElementValues_
							});
						}

						void CreateAblationLowestTC1Report_()
						{
							if(ablationData_[TestParameter.Ablation_TC1] == null || ablationData_[TestParameter.Ablation_TC1].Count == 0)
							{
								return;
							}
							var ablationLowestTC1ElementValues_ = new string[ablationCount_ + 1][];
							ablationLowestTC1ElementValues_[0] = new[] { AblationLowestTC1Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_TC1]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationLowestTC1ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									NAText,
									ablationData_[TestParameter.Ablation_TC1]?.ElementAt(indexAblation_).Item1.ToString(ZeroDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = AblationLowestTC1Text,
								ElementValue = ablationLowestTC1ElementValues_
							});
						}

						void CreateAblationTimeToMinus50Report()
						{
							if(ablationData_[TestParameter.FiftyDegree_Time] == null || ablationData_[TestParameter.FiftyDegree_Time].Count == 0)
							{
								return;
							}
							var ablationTimeTo50Values_ = new string[ablationCount_ + 1][];
							ablationTimeTo50Values_[0] = new[] { AblationTimeTo50Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.FiftyDegree_Time]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationTimeTo50Values_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									NAText,
									ablationData_[TestParameter.FiftyDegree_Time]?.ElementAt(indexAblation_).Item1.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = AblationTimeTo50Text,
								ElementValue = ablationTimeTo50Values_
							});
						}

						void CreateAblationPWM1Report_()
						{
							if(ablationData_[TestParameter.Ablation_PWM1] == null || ablationData_[TestParameter.Ablation_PWM1].Count == 0)
							{
								return;
							}
							var ablationPWM1Values_ = new string[ablationCount_ + 1][];
							ablationPWM1Values_[0] = new[] { AblationPWM1Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_PWM1]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationPWM1Values_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									NAText,
									ablationData_[TestParameter.Ablation_PWM1]?.ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = AblationPWM1Text,
								ElementValue = ablationPWM1Values_
							});
						}

						void CreateAblationPWM2Report_()
						{
							if(ablationData_[TestParameter.Ablation_PWM2] == null || ablationData_[TestParameter.Ablation_PWM2].Count == 0)
							{
								return;
							}
							var ablationPWM2Values_ = new string[ablationCount_ + 1][];
							ablationPWM2Values_[0] = new[] { AblationPWM2Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_PWM2]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								var validatedResult_ = ablationData_[TestParameter.Ablation_PWM2]?.ElementAt(indexAblation_).Item2;
								ablationPWM2Values_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									validatedResult_ == null ? NAText : AblationPWM2TextRule1,
									ablationData_[TestParameter.Ablation_PWM2]?.ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									validatedResult_ == null ? NAText : validatedResult_ == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Ablation_PWM2];
							var displaytitle = res != null ? (res.Value ? $"{AblationPWM2Text}-{imagePath_}{PassedImage}" : $"{AblationPWM2Text}-{imagePath_}{FailedImage}") : $"{AblationPWM2Text}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationPWM2Values_
							});
						}

						void CreateAblationPT3Report_()
						{
							if(ablationData_[TestParameter.Ablation_PT3] == null || ablationData_[TestParameter.Ablation_PT3].Count == 0)
							{
								return;
							}
							var ablationPT3ElementValues_ = new string[ablationCount_ + 1][];
							ablationPT3ElementValues_[0] = new[] { AblationPT3Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_PT3]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationPT3ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									NAText,
									ablationData_[TestParameter.Ablation_PT3].ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = AblationPT3Text,
								ElementValue = ablationPT3ElementValues_
							});
						}

						void CreateAblationPT4Report_()
						{
							if(ablationData_[TestParameter.Ablation_PT4] == null || ablationData_[TestParameter.Ablation_PT4].Count == 0)
							{
								return;
							}
							var ablationPT4ElementValues_ = new string[ablationCount_ + 1][];
							ablationPT4ElementValues_[0] = new[] { AblationPT4Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_PT4]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationPT4ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									NAText,
									ablationData_[TestParameter.Ablation_PT4].ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = AblationPT4Text,
								ElementValue = ablationPT4ElementValues_
							});
						}

						void CreateAblationPT5Report_()
						{
							if(ablationData_[TestParameter.Ablation_PT5] == null || ablationData_[TestParameter.Ablation_PT5].Count == 0)
							{
								return;
							}
							var ablationPT5ElementValues_ = new string[ablationCount_ + 1][];
							ablationPT5ElementValues_[0] = new[] { AblationPT5Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_PT5]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationPT5ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									NAText,
									ablationData_[TestParameter.Ablation_PT5].ElementAt(indexAblation_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = AblationPT5Text,
								ElementValue = ablationPT5ElementValues_
							});
						}

						void CreateAblationTS1Report_()
						{
							if(ablationData_[TestParameter.Ablation_TS1] == null || ablationData_[TestParameter.Ablation_TS1].Count == 0)
							{
								return;
							}
							var ablationTS1ElementValues_ = new string[ablationCount_ + 1][];
							ablationTS1ElementValues_[0] = new[] { AblationTS1Text, ExpectedText, ActualText, ResultText };
							for(int indexAblation_ = 0; indexAblation_ < ablationCount_; indexAblation_++)
							{
								if(ablationData_[TestParameter.Ablation_TS1]?.ElementAt(indexAblation_) == null)
								{
									continue;
								}
								ablationTS1ElementValues_[indexAblation_ + 1] = new[]
								{
									$"{AblationIndexText}{indexAblation_ + 1}",
									AblationTS1ThresholdText,
									ablationData_[TestParameter.Ablation_TS1].ElementAt(indexAblation_).Item1.ToString(OneDecimalPlace, CultureInfo.InvariantCulture),
									ablationData_[TestParameter.Ablation_TS1].ElementAt(indexAblation_).Item2 == true ? PassedMessage : FailedMessage
								};
							}
							bool? res = ablationResults[TestParameter.Ablation_TS1];
							var displaytitle = res != null ? (res.Value ? $"{AblationTS1Text}-{imagePath_}{PassedImage}" : $"{AblationTS1Text}-{imagePath_}{FailedImage}") : $"{AblationTS1Text}";
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = res == null ? ElementTypeTable : ElementTypeTableImageResult,
								ElementDispalyName = displaytitle,
								ElementValue = ablationTS1ElementValues_
							});
						}
					}

					void CreateThawingTestReport_()
					{
						var thawingData_ = testResult.AblationReport.Result.AblationResult?[AblationTestState.THAWING];
						if(thawingData_ == null)
						{
							return;
						}
						var thawingCount_ = thawingData_[TestParameter.Thawing_PT3].Count;
						if(thawingCount_ == 0)
						{
							return;
						}

						var thawingResults = GetThawingTestResults();
						CreateNewPage_();
						CreateThawingTitle_();
						CreateThawingPT3Report_();
						CreateThawingPT4Report_();
						CreateThawingPT5Report_();

						CreateNewPage_();
						CreateThawingPWM1Report_();
						CreateThawingPWM2Report_();

						IDictionary<TestParameter, bool?> GetThawingTestResults()
						{
							var resultdict = new Dictionary<TestParameter, bool?>();
							foreach (KeyValuePair<TestParameter, List<(double, bool?, double?)>> entry in thawingData_)
							{
								bool? result = entry.Value.All(z => z.Item2 == null) ? null : (bool?)!entry.Value.Select(x => x.Item2).Any(y => y.Value == false);
								resultdict.Add(entry.Key, result);
							}
							thawingpassed = !resultdict.Any(x => x.Value == false);
							return resultdict;
						}

						void CreateThawingTitle_()
						{
							var thawingTitleValues_ = new string[2][];
							thawingTitleValues_[0] = new[] { WhiteSpace, WhiteSpace };
							thawingTitleValues_[1] = new[] { WhiteSpace, WhiteSpace };
							var thawingpassedimage = thawingpassed ? PassedImage : FailedImage;
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTableImage,
								ElementDispalyName = $"{ThawingTestTitleText}-{imagePath_}{thawingpassedimage}",
								ElementValue = thawingTitleValues_
							});
						}

						void CreateThawingPT3Report_()
						{
							if(thawingData_[TestParameter.Thawing_PT3] == null || thawingData_[TestParameter.Thawing_PT3].Count == 0)
							{
								return;
							}
							var thawingPT3ElementValues_ = new string[thawingCount_ + 1][];
							thawingPT3ElementValues_[0] = new[] { ThawingPT3Text, ExpectedText, ActualText, ResultText };
							for(int indexThawing_ = 0; indexThawing_ < thawingCount_; indexThawing_++)
							{
								if(thawingData_[TestParameter.Thawing_PT3]?.ElementAt(indexThawing_) == null)
								{
									continue;
								}
								thawingPT3ElementValues_[indexThawing_ + 1] = new[]
								{
									$"{ThawingIndexText}{indexThawing_ + 1}",
									NAText,
									thawingData_[TestParameter.Thawing_PT3].ElementAt(indexThawing_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = ThawingPT3Text,
								ElementValue = thawingPT3ElementValues_
							});
						}

						void CreateThawingPT4Report_()
						{
							if(thawingData_[TestParameter.Thawing_PT4] == null || thawingData_[TestParameter.Thawing_PT4].Count == 0)
							{
								return;
							}
							var thawingPT4ElementValues_ = new string[thawingCount_ + 1][];
							thawingPT4ElementValues_[0] = new[] { ThawingPT4Text, ExpectedText, ActualText, ResultText };
							for(int indexThawing_ = 0; indexThawing_ < thawingCount_; indexThawing_++)
							{
								if(thawingData_[TestParameter.Thawing_PT4]?.ElementAt(indexThawing_) == null)
								{
									continue;
								}
								thawingPT4ElementValues_[indexThawing_ + 1] = new[]
								{
									$"{ThawingIndexText}{indexThawing_ + 1}",
									NAText,
									thawingData_[TestParameter.Thawing_PT4]?.ElementAt(indexThawing_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = ThawingPT4Text,
								ElementValue = thawingPT4ElementValues_
							});
						}

						void CreateThawingPT5Report_()
						{
							if(thawingData_[TestParameter.Thawing_PT5] == null || thawingData_[TestParameter.Thawing_PT5].Count == 0)
							{
								return;
							}
							var thawingPT5ElementValues_ = new string[thawingCount_ + 1][];
							thawingPT5ElementValues_[0] = new[] { ThawingPT5Text, ExpectedText, ActualText, ResultText };
							for(int indexThawing_ = 0; indexThawing_ < thawingCount_; indexThawing_++)
							{
								if(thawingData_[TestParameter.Thawing_PT5]?.ElementAt(indexThawing_) == null)
								{
									continue;
								}
								thawingPT5ElementValues_[indexThawing_ + 1] = new[]
								{
									$"{ThawingIndexText}{indexThawing_ + 1}",
									NAText,
									thawingData_[TestParameter.Thawing_PT5].ElementAt(indexThawing_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = ThawingPT5Text,
								ElementValue = thawingPT5ElementValues_
							});
						}

						void CreateThawingPWM1Report_()
						{
							if(thawingData_[TestParameter.Thawing_PWM1] == null || thawingData_[TestParameter.Thawing_PWM1].Count == 0)
							{
								return;
							}
							thawingCount_ = thawingData_[TestParameter.Thawing_PWM1].Count;
							var thawingPWM1ElementValues_ = new string[thawingCount_ + 1][];
							thawingPWM1ElementValues_[0] = new[] { ThawingPWM1Text, ExpectedText, ActualText, ResultText };
							for(int indexThawing_ = 0; indexThawing_ < thawingCount_; indexThawing_++)
							{
								if(thawingData_[TestParameter.Thawing_PWM1]?.ElementAt(indexThawing_) == null)
								{
									continue;
								}
								thawingPWM1ElementValues_[indexThawing_ + 1] = new[]
								{
									$"{ThawingIndexText}{indexThawing_ + 1}",
									NAText,
									thawingData_[TestParameter.Thawing_PWM1].ElementAt(indexThawing_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = ThawingPWM1Text,
								ElementValue = thawingPWM1ElementValues_
							});
						}

						void CreateThawingPWM2Report_()
						{
							if(thawingData_[TestParameter.Thawing_PWM2] == null || thawingData_[TestParameter.Thawing_PWM2].Count == 0)
							{
								return;
							}
							thawingCount_ = thawingData_[TestParameter.Thawing_PWM2].Count;
							var thawingPWM2ElementValues_ = new string[thawingCount_ + 1][];
							thawingPWM2ElementValues_[0] = new[] { ThawingPWM2Text, ExpectedText, ActualText, ResultText };
							for(int indexThawing_ = 0; indexThawing_ < thawingCount_; indexThawing_++)
							{
								if(thawingData_[TestParameter.Thawing_PWM2]?.ElementAt(indexThawing_) == null)
								{
									continue;
								}
								thawingPWM2ElementValues_[indexThawing_ + 1] = new[]
								{
									$"{ThawingIndexText}{indexThawing_ + 1}",
									NAText,
									thawingData_[TestParameter.Thawing_PWM2].ElementAt(indexThawing_).Item1.ToString(TwoDecimalPlace, CultureInfo.InvariantCulture),
									NAText
								};
							}
							testResultPDF_.Add(new PDFElementsTable
							{
								ElementType = ElementTypeTable,
								ElementDispalyName = ThawingPWM2Text,
								ElementValue = thawingPWM2ElementValues_
							});
						}
					}
				}
			}

			void CreateRetryRationaleReport_()
			{
				if(testResult.RationaleReport?.Result?.RationaleList == null)
				{
					return;
				}
				var rationaleCount_ = testResult.RationaleReport.Result.RationaleList.Count;
				if(rationaleCount_ <= 0)
				{
					return;
				}

				CreateNewPage_();
				CreateRetryRationaleTitle_();
				CreateRationaleReport_();

				void CreateRetryRationaleTitle_()
				{
					var rationaleValues_ = new string[2][];
					rationaleValues_[0] = new[] { WhiteSpace, WhiteSpace };
					rationaleValues_[1] = new[] { WhiteSpace, WhiteSpace };
					testResultPDF_.Add(new PDFElementsTable()
					{
						ElementType = ElementTypeTableSmall,
						ElementDispalyName = RetryRationaleTestText,
						ElementValue = rationaleValues_
					});
				}
				void CreateRationaleReport_()
				{
					var rationaleElementValues_ = new string[rationaleCount_ + 1][];
					rationaleElementValues_[0] = new[] { TestNameText, RationaleText };
					for(int index_ = 0; index_ < rationaleCount_; index_++)
					{
						rationaleElementValues_[index_ + 1] = new[]
						{
							testResult.RationaleReport.Result.RationaleList[index_].Item1.ToString(CultureInfo.InvariantCulture),
							testResult.RationaleReport.Result.RationaleList[index_].Item2.ToString(CultureInfo.InvariantCulture)
						};
					}
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTable,
						ElementDispalyName = WhiteSpace,
						ElementValue = rationaleElementValues_
					});
				}
			}

			void CreateErrorMessageReport_()
			{
				if(testResult.ErrorReport?.Result?.ErrorList == null)
				{
					return;
				}

				var errorCount_ = testResult.ErrorReport.Result.ErrorList.Count;
				if(errorCount_ <= 0)
				{
					return;
				}

				CreateNewPage_();
				CreateErrorMessageTitle_();
				CreateErrorReport_();

				void CreateErrorMessageTitle_()
				{
					var errorValues_ = new string[2][];
					errorValues_[0] = new[] { WhiteSpace, WhiteSpace };
					errorValues_[1] = new[] { WhiteSpace, WhiteSpace };
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTableSmall,
						ElementDispalyName = ErrorMessageText,
						ElementValue = errorValues_
					});
				}
				void CreateErrorReport_()
				{
					var errorElementValues_ = new string[errorCount_ + 1][];
					errorElementValues_[0] = new[] { ErrorSummary, ErrorText };
					for(int index_ = 0; index_ < errorCount_; index_++)
					{
						errorElementValues_[index_ + 1] = new[]
						{
							testResult.ErrorReport.Result.ErrorList[index_].Item1.ToString(CultureInfo.InstalledUICulture),
							testResult.ErrorReport.Result.ErrorList[index_].Item2.ToString(CultureInfo.InstalledUICulture)
						};
					}
					testResultPDF_.Add(new PDFElementsTable
					{
						ElementType = ElementTypeTable,
						ElementDispalyName = WhiteSpace,
						ElementValue = errorElementValues_
					});
				}
			}

			void GenerateTestReport_()
			{
				pdfTemplate_.SaveToPDFTemplate(reportFileName, testResultPDF_, string.Empty, DateTime.Today.ToString(DateTimeFormat), PageFieldText, string.Empty, $"{testResult.ConsoleSerialNumber ?? string.Empty}", true);
			}
		}
	}
}