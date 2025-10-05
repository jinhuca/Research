-- Delete Translations
DELETE FROM Translations
DBCC CHECKIDENT (Translations, RESEED, 1)

-- Languages
DELETE FROM Languages
DBCC CHECKIDENT (Languages, RESEED, 1)
INSERT INTO Languages VALUES ('English')
INSERT INTO Languages VALUES ('German')
INSERT INTO Languages VALUES ('French')

-- Delete GUIFields
DELETE FROM GUIFields
DBCC CHECKIDENT (GUIFields, RESEED, 1)

-- *****************************************************************************************
-- GUI FIELDS - BEGIN
-- *****************************************************************************************
-- CRYOTHERAPY SCREEN
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('StartButton') -- ID 1
INSERT INTO GUIFields VALUES ('StopButton') -- ID 2
INSERT INTO GUIFields VALUES ('ConnectButton') -- ID 3
INSERT INTO GUIFields VALUES ('DisconnectButton') -- ID 4
INSERT INTO GUIFields VALUES ('VeinIsolationTextBlock') -- ID 5
INSERT INTO GUIFields VALUES ('NotificationTextBlock') -- ID 6
INSERT INTO GUIFields VALUES ('NotesTextBlock') -- ID 7
INSERT INTO GUIFields VALUES ('DeflateAtThawTextBlock') -- ID 8
INSERT INTO GUIFields VALUES ('STATUSLabel') -- ID 9
INSERT INTO GUIFields VALUES ('IDLELabel') -- ID 10
INSERT INTO GUIFields VALUES ('READYLabel') -- ID 11
INSERT INTO GUIFields VALUES ('INFLATIONLabel') -- ID 12
INSERT INTO GUIFields VALUES ('ABLATIONLabel') -- ID 13
INSERT INTO GUIFields VALUES ('THAWINGLabel') -- ID 14
INSERT INTO GUIFields VALUES ('ABLATIONSUMMARYLabel') -- ID 15
INSERT INTO GUIFields VALUES ('AblationSiteLabel') -- ID 16
INSERT INTO GUIFields VALUES ('AblationsLabel') -- ID 17
INSERT INTO GUIFields VALUES ('DurationInSecLabel') -- ID 18
INSERT INTO GUIFields VALUES ('RSPVLabel') -- ID 19
INSERT INTO GUIFields VALUES ('RIPVLabel') -- ID 20
INSERT INTO GUIFields VALUES ('LSPVLabel') -- ID 21
INSERT INTO GUIFields VALUES ('LIPVLabel') -- ID 22
INSERT INTO GUIFields VALUES ('OTHERLabel') -- ID 23
INSERT INTO GUIFields VALUES ('TotalLabel') -- ID 24
INSERT INTO GUIFields VALUES ('ElapsedTimeLabel') -- ID 25
INSERT INTO GUIFields VALUES ('minLabel') -- ID 26
INSERT INTO GUIFields VALUES ('CurrentTimeLabel') -- ID 27
INSERT INTO GUIFields VALUES ('TEMPERATURELabel') -- ID 28
INSERT INTO GUIFields VALUES ('AblationTimeLabel') -- ID 29
INSERT INTO GUIFields VALUES ('TIMERSAndRATESLabel') -- ID 30
INSERT INTO GUIFields VALUES ('ALERTSAndINDICATORSLabel') -- ID 31
INSERT INTO GUIFields VALUES ('SystemINFOLabel') -- ID 32
INSERT INTO GUIFields VALUES ('TEMPERATUREAndRATELabel') -- ID 33
INSERT INTO GUIFields VALUES ('MinimumTEMPERATURELabel') -- ID 34
INSERT INTO GUIFields VALUES ('CoolingTimeToLabel') -- ID 35
INSERT INTO GUIFields VALUES ('TimeToEffectLabel') -- ID 36
INSERT INTO GUIFields VALUES ('ThawTimeToLabel') -- ID 37
INSERT INTO GUIFields VALUES ('DiaphragmLabel') -- ID 38
INSERT INTO GUIFields VALUES ('MovementLabel') -- ID 39
INSERT INTO GUIFields VALUES ('ZoomLabel') -- ID 40
INSERT INTO GUIFields VALUES ('NoPacingDetectedLabel') -- ID 41
INSERT INTO GUIFields VALUES ('ForReferenceOnlyNeverRelySolelyOnTheseIndicatorsLabel') -- ID 42
INSERT INTO GUIFields VALUES ('ESOPHAGUSLabel') -- ID 43
INSERT INTO GUIFields VALUES ('TEMPERATURELabel1') -- ID 44
INSERT INTO GUIFields VALUES ('TemperatureRangeLabel') -- ID 45
INSERT INTO GUIFields VALUES ('FlowLabel') -- ID 46
INSERT INTO GUIFields VALUES ('PRESSURELabel') -- ID 47
INSERT INTO GUIFields VALUES ('BalloonLabel') -- ID 48
INSERT INTO GUIFields VALUES ('TimerLabel') -- ID 49
INSERT INTO GUIFields VALUES ('SecLabel') -- ID 50
INSERT INTO GUIFields VALUES ('TreatmentLabel') -- ID 51
INSERT INTO GUIFields VALUES ('TreatmentNumberOf') -- ID 52
INSERT INTO GUIFields VALUES ('WaitSystemIsInitializing') -- ID 53
-- *****************************************************************************************
-- TREATMENT RECORDS SCREEN - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('TreatmentRecordTitleLabel') -- ID 54
INSERT INTO GUIFields VALUES ('PatientInfoLabel') -- ID 55
INSERT INTO GUIFields VALUES ('PatientNameLabel') -- ID 56
INSERT INTO GUIFields VALUES ('PatientGenderLabel') -- ID 57
INSERT INTO GUIFields VALUES ('PatientBirthDateLabel') -- ID 58 
INSERT INTO GUIFields VALUES ('PatientIdNumberLabel') -- ID 59
INSERT INTO GUIFields VALUES ('ProcedureInfoLabel') -- ID 60
INSERT INTO GUIFields VALUES ('PhysicianNameLabel') -- ID 61
INSERT INTO GUIFields VALUES ('CatheterUsedLabel') -- ID 62
INSERT INTO GUIFields VALUES ('ProcedureDateLabel') -- ID 63
INSERT INTO GUIFields VALUES ('EsophagusTemperatureLabel') -- ID 64
INSERT INTO GUIFields VALUES ('DiaphragmMovementLabel') -- ID 65
INSERT INTO GUIFields VALUES ('BalloonPressureLabel') -- ID 66
INSERT INTO GUIFields VALUES ('ProcedureRecordsLabel') -- ID 67
INSERT INTO GUIFields VALUES ('ExportProcedureButton') -- ID 68
INSERT INTO GUIFields VALUES ('SaveEngineeringDataInProgressLabel') -- ID 69
-- *****************************************************************************************
-- SUMMARY REPORT SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('SummaryReportLabel') -- ID 70
INSERT INTO GUIFields VALUES ('TreatmentInfoLabel') -- ID 71 
INSERT INTO GUIFields VALUES ('DiagnosisLabel') -- ID 72
INSERT INTO GUIFields VALUES ('OutcomeLabel') -- ID 73
-- *****************************************************************************************
-- CHANGE TANK SCREEN   - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('TankReplacementLabel') -- ID 74
INSERT INTO GUIFields VALUES ('CloseTankLabel') -- ID 75
INSERT INTO GUIFields VALUES ('WaitLabel') -- ID 76
INSERT INTO GUIFields VALUES ('ReplaceTankLabel') -- ID 77
INSERT INTO GUIFields VALUES ('OpenTankLabel') -- ID 78
INSERT INTO GUIFields VALUES ('FollowInstructionsLabel') -- ID 79
INSERT INTO GUIFields VALUES ('ChangeTankSuccessLabel') -- ID 80
INSERT INTO GUIFields VALUES ('NextButton') -- ID 81
INSERT INTO GUIFields VALUES ('FinishButton') -- ID 82
INSERT INTO GUIFields VALUES ('CancelButton') -- ID 83
INSERT INTO GUIFields VALUES ('ChangeDateLabel') -- ID 84
INSERT INTO GUIFields VALUES ('WeightAtChangeLabel') -- ID 85
INSERT INTO GUIFields VALUES ('CurrentWeightLabel') -- ID 86
INSERT INTO GUIFields VALUES ('SelectTankReplacementTypeLabel') -- ID 87
INSERT INTO GUIFields VALUES ('TenPoundsLabel') -- ID 88
INSERT INTO GUIFields VALUES ('FifteenPoundsLabel') -- ID 89
-- *****************************************************************************************
-- MANAGE USERS SCREEN   - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('NewUserLabel') -- ID 90
INSERT INTO GUIFields VALUES ('NewDoctorLabel') -- ID 91
INSERT INTO GUIFields VALUES ('EditUserLabel') -- ID 92
INSERT INTO GUIFields VALUES ('EditDoctorLabel') -- ID 93
INSERT INTO GUIFields VALUES ('DeleteUserLabel') -- ID 94
INSERT INTO GUIFields VALUES ('DeleteDoctorLabel') -- ID 95
INSERT INTO GUIFields VALUES ('ResetPasswordLabel') -- ID 96
INSERT INTO GUIFields VALUES ('ReturnToSettingsButton') -- ID 97
INSERT INTO GUIFields VALUES ('UserListLabel') -- ID 98
INSERT INTO GUIFields VALUES ('ManageUsersLabel') -- ID 99
INSERT INTO GUIFields VALUES ('UsernameLabel') -- ID 100
INSERT INTO GUIFields VALUES ('DoctorsNameLabel') -- ID 101
INSERT INTO GUIFields VALUES ('CurrentPasswordLabel') -- ID 102
INSERT INTO GUIFields VALUES ('PasswordLabel') -- ID 103
INSERT INTO GUIFields VALUES ('ConfirmPasswordLabel') -- ID 104
INSERT INTO GUIFields VALUES ('PasswordsDontMatchLabel') -- ID 105
INSERT INTO GUIFields VALUES ('PasswordInvalidLabel') -- ID 106
INSERT INTO GUIFields VALUES ('AdminLabel') -- ID 107
INSERT INTO GUIFields VALUES ('UsernameAlreadyExistsText') -- ID 108
INSERT INTO GUIFields VALUES ('UsernameAlreadyExistsTitle') -- ID 109
INSERT INTO GUIFields VALUES ('UsernameAlreadyExistsInactive') -- ID 110
INSERT INTO GUIFields VALUES ('TheUsername') -- ID 111
INSERT INTO GUIFields VALUES ('ReactivateUserTitle') -- ID 112
INSERT INTO GUIFields VALUES ('ThePhysicianName') -- ID 113
INSERT INTO GUIFields VALUES ('DeleteUserMessage') -- ID 114
INSERT INTO GUIFields VALUES ('DeleteUserTitle') -- ID 115

-- *****************************************************************************************
-- Home  SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('ShutDownLabel') -- ID 116
INSERT INTO GUIFields VALUES ('ChangeTankLabel') -- ID 117
INSERT INTO GUIFields VALUES ('CryoTherapyLabel') -- ID 118
INSERT INTO GUIFields VALUES ('RecordsLabel') -- ID 119
INSERT INTO GUIFields VALUES ('SettingsLabel') -- ID 120

-- *****************************************************************************************
-- SETTINGS SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('TimerSettingsPreferencesLabel') -- ID 121
INSERT INTO GUIFields VALUES ('CoolingTimerToTextBlock') -- ID 122
INSERT INTO GUIFields VALUES ('ThawTimerToTextBlock') -- ID 123
INSERT INTO GUIFields VALUES ('AblationTimerTextBlock') -- ID 124
INSERT INTO GUIFields VALUES ('SecondsTextBlock') -- ID 125
INSERT INTO GUIFields VALUES ('ChartTypeTextBlock') -- ID 126
INSERT INTO GUIFields VALUES ('CurveColorTextBlock') -- ID 127

INSERT INTO GUIFields VALUES ('AlertSettingsPreferencesLabel') -- ID 128
INSERT INTO GUIFields VALUES ('LowAblationTemperatureTextBlock') -- ID 129
INSERT INTO GUIFields VALUES ('HighAblationTemperatureTextBlock') -- ID 130
INSERT INTO GUIFields VALUES ('EsophagusTemperatureTextBlock') -- ID 131
INSERT INTO GUIFields VALUES ('DiaphragmSensorLimitTextBlock') -- ID 132
INSERT INTO GUIFields VALUES ('DiaphragmSensorGainTextBlock') -- ID 133

INSERT INTO GUIFields VALUES ('SystemSettingsLabel') -- ID 134
INSERT INTO GUIFields VALUES ('InflateSpeedTextBlock') -- ID 135
INSERT INTO GUIFields VALUES ('FastTextBlock') -- ID 136
INSERT INTO GUIFields VALUES ('SlowTextBox') -- ID 137
INSERT INTO GUIFields VALUES ('OnTextBlock') -- ID 138
INSERT INTO GUIFields VALUES ('OffTextBlock') -- ID 139
INSERT INTO GUIFields VALUES ('DMSTextBlock') -- ID 140
INSERT INTO GUIFields VALUES ('LineTextBlock') -- ID 141
INSERT INTO GUIFields VALUES ('AreaTextBlock') -- ID 142
INSERT INTO GUIFields VALUES ('RefrigerantLevelTextBlock') -- ID 143
INSERT INTO GUIFields VALUES ('LbsTextBlock') -- ID 144
INSERT INTO GUIFields VALUES ('MinTextBlock') -- ID 145

-- *****************************************************************************************
-- LOGIN SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('LoginLabel') -- ID 146
INSERT INTO GUIFields VALUES ('WrongUsernameOrPasswordLabel') -- ID 147
INSERT INTO GUIFields VALUES ('OkButton') -- ID 148

-- *****************************************************************************************
-- PATIENT SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('PatientInformationLabel') -- ID 149
INSERT INTO GUIFields VALUES ('PatientIdLabel') -- ID 150
INSERT INTO GUIFields VALUES ('FirstNameLabel') -- ID 151
INSERT INTO GUIFields VALUES ('LastNameLabel') -- ID 152
INSERT INTO GUIFields VALUES ('GenderLabel') -- ID 153
INSERT INTO GUIFields VALUES ('MaleLabel') -- ID 154
INSERT INTO GUIFields VALUES ('FemaleLabel') -- ID 155
INSERT INTO GUIFields VALUES ('BirthDateLabel') -- ID 156
INSERT INTO GUIFields VALUES ('DayLabel') -- ID 157
INSERT INTO GUIFields VALUES ('MonthLabel') -- ID 158
INSERT INTO GUIFields VALUES ('YearLabel') -- ID 159
INSERT INTO GUIFields VALUES ('PhysicianLabel') -- ID 160

-- *****************************************************************************************
-- GENERIC SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('YESButton') -- ID 161
INSERT INTO GUIFields VALUES ('NOButton') -- ID 162
INSERT INTO GUIFields VALUES ('YesButton') -- ID 163
INSERT INTO GUIFields VALUES ('NoButton') -- ID 164
INSERT INTO GUIFields VALUES ('YESLabel') -- ID 165
INSERT INTO GUIFields VALUES ('NOLabel') -- ID 166
INSERT INTO GUIFields VALUES ('YesLabel') -- ID 167
INSERT INTO GUIFields VALUES ('NoLabel') -- ID 168

-- *****************************************************************************************
-- MESSAGE POPUP SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('SystemMessageLabel') -- ID 169
INSERT INTO GUIFields VALUES ('WarningMessageLabel') -- ID 170
INSERT INTO GUIFields VALUES ('ErrorMessageLabel') -- ID 171

-- *****************************************************************************************
-- SETTINGS MAIN SCREEN  - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('DateTimeLabel') -- ID 172
INSERT INTO GUIFields VALUES ('UserManualLabel') -- ID 173
INSERT INTO GUIFields VALUES ('MaintenanceLabel') -- ID 174
INSERT INTO GUIFields VALUES ('ActionLogLabel') -- ID 175
INSERT INTO GUIFields VALUES ('DaysLabel') -- ID 176
INSERT INTO GUIFields VALUES ('HoursLabel') -- ID 177
INSERT INTO GUIFields VALUES ('MinutesLabel') -- ID 178


-- *****************************************************************************************
-- Add New Fields here - GUI FIELDS
-- *****************************************************************************************
INSERT INTO GUIFields VALUES ('NoPacingDetectedLabelOff') -- ID 179
INSERT INTO GUIFields VALUES ('WeightLabel') -- ID 180
INSERT INTO GUIFields VALUES ('HeightLabel') -- ID 181
INSERT INTO GUIFields VALUES ('ActualAblationTimerTextBlock') -- ID 182
INSERT INTO GUIFields VALUES ('ExpectedVeinIsolationTimeTextBlock') -- ID 183
INSERT INTO GUIFields VALUES ('NewAblationTimerTextBlock') -- ID 184
INSERT INTO GUIFields VALUES ('VeinIsolationLogicLabel') -- ID 185
INSERT INTO GUIFields VALUES ('UpdateVeinIsolationDurationLabel') -- ID 186
INSERT INTO GUIFields VALUES ('NewVeinIsolationDurationLabel') -- ID 187
INSERT INTO GUIFields VALUES ('InvalidDurationLabel') -- ID 188
INSERT INTO GUIFields VALUES ('AblationTimersLabel') -- ID 189
INSERT INTO GUIFields VALUES ('FixedTimerTextBlock') -- ID 190
INSERT INTO GUIFields VALUES ('TTIFixedTimerTextBlock') -- ID 191
INSERT INTO GUIFields VALUES ('TTIDurationTimerTextBlock') -- ID 192
INSERT INTO GUIFields VALUES ('AblationTimerTTIPlusTextBlock') -- ID 193
INSERT INTO GUIFields VALUES ('CryoBalloonTemperatureLabel') -- ID 194
INSERT INTO GUIFields VALUES ('SelectAblationSiteLabel') -- ID 195
INSERT INTO GUIFields VALUES ('ExitPlaybackButton') -- ID 196
INSERT INTO GUIFields VALUES ('PlaybackModeLabel') -- ID 197
INSERT INTO GUIFields VALUES ('CompleteProcedureButton') -- ID 198
INSERT INTO GUIFields VALUES ('ReturnToProcedureButton') -- ID 199
INSERT INTO GUIFields VALUES ('EndProcedureButton') -- ID 200
INSERT INTO GUIFields VALUES ('PlaybackButton') -- ID 201
INSERT INTO GUIFields VALUES ('CaseDateLabel') -- ID 202
INSERT INTO GUIFields VALUES ('SaveToUSBDriveLabel') -- ID 203
INSERT INTO GUIFields VALUES ('VolumeLabel') -- ID 204
INSERT INTO GUIFields VALUES ('NameLabel') -- ID 205
INSERT INTO GUIFields VALUES ('DriveFormatLabel') -- ID 206
INSERT INTO GUIFields VALUES ('FreeSpaceLabel') -- ID 207
INSERT INTO GUIFields VALUES ('TotalSizeLabel') -- ID 208
INSERT INTO GUIFields VALUES ('FileNameLabel') -- ID 209
INSERT INTO GUIFields VALUES ('FileTypeLabel') -- ID 210
INSERT INTO GUIFields VALUES ('BackToTreatmentRecordButton') -- ID 211
INSERT INTO GUIFields VALUES ('UserLabel') -- ID 212
INSERT INTO GUIFields VALUES ('HomeLabel') -- ID 213
INSERT INTO GUIFields VALUES ('AccessTypeLabel') -- ID 214
INSERT INTO GUIFields VALUES ('ThenTextBlock') -- ID 215
INSERT INTO GUIFields VALUES ('ElseTextBlock') -- ID 216
INSERT INTO GUIFields VALUES ('SelectADateLabel') -- ID 217
INSERT INTO GUIFields VALUES ('QualifiedPersonLabel') -- ID 218
INSERT INTO GUIFields VALUES ('MessageLabel') -- ID 219
INSERT INTO GUIFields VALUES ('ResetSystemButton') -- ID 220
INSERT INTO GUIFields VALUES ('CloseButton') -- ID 221
INSERT INTO GUIFields VALUES ('ActionRequiredLabel') -- ID 222
INSERT INTO GUIFields VALUES ('UpdateDurationLabel') -- ID 223
INSERT INTO GUIFields VALUES ('NewDurationLabel') -- ID 224
INSERT INTO GUIFields VALUES ('UpdateAblationSiteWarningLabel') -- ID 225
INSERT INTO GUIFields VALUES ('MultipleFilesSelectedLabel') -- ID 226
INSERT INTO GUIFields VALUES ('ErrorLabel') -- ID 227
INSERT INTO GUIFields VALUES ('InvalidDMSThresholdLabel') -- ID 228
INSERT INTO GUIFields VALUES ('MinLabel') -- ID 229
INSERT INTO GUIFields VALUES ('MaxLabel') -- ID 230
INSERT INTO GUIFields VALUES ('AudioAlertTextBlock') -- ID 231
--  *****************************************************************************************
-- GUI FIELDS - END
-- *****************************************************************************************





-- *****************************************************************************************
-- ENGLISH TRANSLATION - BEGIN
-- *****************************************************************************************
-- CRYOTHERAPY SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('START','1','1')  -- StartButton
INSERT INTO Translations VALUES ('STOP','1','2')  -- StopButton
INSERT INTO Translations VALUES ('VACUUM ON','1','3')  -- ConnectButton
INSERT INTO Translations VALUES ('VACUUM OFF','1','4')  -- DisconnectButton
INSERT INTO Translations VALUES ('Vein Isolated','1','5')  -- VeinIsolationTextBlock
INSERT INTO Translations VALUES ('Settings','1','6')  -- NotificationTextBlock
INSERT INTO Translations VALUES ('Notes','1','7')  -- NotesTextBlock
INSERT INTO Translations VALUES ('Deflate at Thaw','1','8')  -- DeflateAtThawTextBlock
INSERT INTO Translations VALUES ('STATUS:','1','9')  -- STATUSLabel
INSERT INTO Translations VALUES ('IDLE','1','10')  -- IDLELabel
INSERT INTO Translations VALUES ('READY','1','11') -- READYLabel
INSERT INTO Translations VALUES ('INFLATION','1','12')  -- INFLATIONLabel
INSERT INTO Translations VALUES ('ABLATION','1','13')  -- ABLATIONLabel
INSERT INTO Translations VALUES ('THAWING','1','14')  -- THAWINGLabel
INSERT INTO Translations VALUES ('ABLATION SUMMARY','1','15')  -- ABLATIONSUMMARYLabel
INSERT INTO Translations VALUES ('Ablation Site','1','16')  -- AblationSiteLabel
INSERT INTO Translations VALUES ('Ablations','1','17')  -- AblationsLabel
INSERT INTO Translations VALUES ('Duration (sec)','1','18')  -- DurationInSecLabel
INSERT INTO Translations VALUES ('RSPV','1','19')  -- RSPVLabel
INSERT INTO Translations VALUES ('RIPV','1','20')  -- RIPVLabel
INSERT INTO Translations VALUES ('LSPV','1','21')  -- LSPVLabel
INSERT INTO Translations VALUES ('LIPV','1','22')  -- LIPVLabel
INSERT INTO Translations VALUES ('OTHER','1','23')  -- OTHERLabel
INSERT INTO Translations VALUES ('Total:','1','24')  -- TotalLabel
INSERT INTO Translations VALUES ('In Body Time:','1','25')  -- ElapsedTimeLabel
INSERT INTO Translations VALUES ('min','1','26')  -- minLabel
INSERT INTO Translations VALUES ('Current Time:','1','27')  -- CurrentTimeLabel
INSERT INTO Translations VALUES ('TEMPERATURE','1','28')  -- TEMPERATURELabel
INSERT INTO Translations VALUES ('ABLATION TIME','1','29')  -- AblationTimeLabel
INSERT INTO Translations VALUES ('TIMERS & RATES','1','30')  -- TIMERSAndRATESLabel
INSERT INTO Translations VALUES ('NOTIFICATIONS & INDICATORS','1','31')  -- ALERTSAndINDICATORSLabel
INSERT INTO Translations VALUES ('SYSTEM INFO','1','32')  -- SystemINFOLabel
INSERT INTO Translations VALUES ('TEMPERATURE RATE','1','33')  -- TEMPERATUREAndRATELabel
INSERT INTO Translations VALUES ('MINIMUM TEMPERATURE','1','34')  -- MinimumTEMPERATURELabel
INSERT INTO Translations VALUES ('COOLING TIME TO','1','35')  -- CoolingTimeToLabel
INSERT INTO Translations VALUES ('TIME TO EFFECT','1','36')  -- TimeToEffectLabel
INSERT INTO Translations VALUES ('THAW TIME TO','1','37')  -- ThawTimeToLabel
INSERT INTO Translations VALUES ('DIAPHRAGM','1','38')  -- DiaphragmLabel
INSERT INTO Translations VALUES ('MOVEMENT','1','39')  -- MovementLabel
INSERT INTO Translations VALUES ('Zoom:','1','40')  -- ZoomLabel
INSERT INTO Translations VALUES ('NO PACING DETECTED','1','41')  -- NoPacingDetectedLabel
INSERT INTO Translations VALUES ('For reference only. Never rely solely on these indicators','1','42')  -- ForReferenceOnlyNeverRelySolelyOnTheseIndicatorsLabel
INSERT INTO Translations VALUES ('ESOPHAGUS','1','43')  -- ESOPHAGUSLabel
INSERT INTO Translations VALUES ('TEMPERATURE','1','44')  -- TEMPERATURELabel1
INSERT INTO Translations VALUES ('10°C to 40°C','1','45')  -- TemperatureRangeLabel
INSERT INTO Translations VALUES ('FLOW:','1','46')  -- FlowLabel
INSERT INTO Translations VALUES ('PRESSURE:','1','47')  -- PRESSURELabel
INSERT INTO Translations VALUES ('BALLOON:','1','48')  -- BalloonLabel
INSERT INTO Translations VALUES ('Ablation Duration','1','49')  -- TimerLabel
INSERT INTO Translations VALUES ('sec','1','50')  -- SecLabel
INSERT INTO Translations VALUES ('Treatment:','1','51')  -- TreatmentLabel
INSERT INTO Translations VALUES ('of','1','52')  -- TreatmentNumberOf
INSERT INTO Translations VALUES ('Wait system is initializing...','1','53')  -- WaitSystemIsInitializing

-- *****************************************************************************************
-- TREATMENT RECORDS SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('TREATMENT RECORDS','1','54')  -- TreatmentRecordTitleLabel
INSERT INTO Translations VALUES ('PATIENT INFO','1','55')  -- PatientInfoLabel
INSERT INTO Translations VALUES ('Patient','1','56')  -- PatientNameLabel
INSERT INTO Translations VALUES ('Gender','1','57')  -- PatientGenderLabel
INSERT INTO Translations VALUES ('Date of Birth','1','58')  -- PatientBirthDateLabel
INSERT INTO Translations VALUES ('ID Number','1','59')  -- PatientIdNumberLabel
INSERT INTO Translations VALUES ('PROCEDURE INFO','1','60')  -- ProcedureInfoLabel
INSERT INTO Translations VALUES ('PHYSICIAN','1','61')  -- PhysicianNameLabel
INSERT INTO Translations VALUES ('Catheter Used','1','62')  -- CatheterUsedLabel
INSERT INTO Translations VALUES ('Procedure Date','1','63')  -- ProcedureDateLabel
INSERT INTO Translations VALUES ('ESOPHAGUS TEMPERATURE','1','64')  -- EsophagusTemperatureLabel
INSERT INTO Translations VALUES ('DIAPHRAGM MOVEMENT','1','65')  -- DiaphragmMovementLabel
INSERT INTO Translations VALUES ('BALLOON PRESSURE','1','66')  -- BalloonPressureLabel
INSERT INTO Translations VALUES ('PROCEDURE RECORDS','1','67')  -- ProcedureRecordsLabel
INSERT INTO Translations VALUES ('Save to USB','1','68')  -- ExportProcedureButton
INSERT INTO Translations VALUES ('Save Engineering Data in progress...','1','69')  -- SaveEngineeringDataInProgressLabel

-- *****************************************************************************************
-- SUMMARY REPORT SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('SUMMARY REPORT','1','70')  -- SummaryReportLabel
INSERT INTO Translations VALUES ('TREATMENT INFO','1','71')  -- TreatmentInfoLabel
INSERT INTO Translations VALUES ('Diagnosis','1','72')  -- DiagnosisLabel
INSERT INTO Translations VALUES ('Outcome','1','73')  -- OutcomeLabel

-- *****************************************************************************************
-- CHANGE TANK SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('TANK REPLACEMENT','1','74')  -- TankReplacementLabel
INSERT INTO Translations VALUES ('Close tank then press Next','1','75')  -- CloseTankLabel
INSERT INTO Translations VALUES ('Please wait while the system purges the line for your safety','1','76')  -- WaitLabel
INSERT INTO Translations VALUES ('Replace the tank','1','77')  -- ReplaceTankLabel
INSERT INTO Translations VALUES ('Open tank','1','78')  -- OpenTankLabel
INSERT INTO Translations VALUES ('Please follow Instructions for safe tank replacement','1','79')  -- FollowInstructionsLabel
INSERT INTO Translations VALUES ('Change tank succeedded!','1','80')  -- ChangeTankSuccessLabel
INSERT INTO Translations VALUES ('Next','1','81')  -- NextButton
INSERT INTO Translations VALUES ('Finish','1','82')  -- FinishButton
INSERT INTO Translations VALUES ('Cancel','1','83')  -- CancelButton
INSERT INTO Translations VALUES ('Change Date :','1','84')  -- ChangeDateLabel
INSERT INTO Translations VALUES ('Weight at Change :','1','85')  -- WeightAtChangeLabel
INSERT INTO Translations VALUES ('Current Weight :','1','86')  -- CurrentWeightLabel
INSERT INTO Translations VALUES ('Select a Replacement Tank Type :','1','87')  -- SelectTankReplacementTypeLabel
INSERT INTO Translations VALUES ('10 pounds','1','88')  -- TenPoundsLabel
INSERT INTO Translations VALUES ('15 pounds','1','89')  -- FifteenPoundsLabel

-- *****************************************************************************************
-- MANAGE USERS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('New User','1','90')  -- NewUserLabel
INSERT INTO Translations VALUES ('New Doctor','1','91')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Edit User','1','92')  -- EditUserLabel
INSERT INTO Translations VALUES ('Edit Doctor','1','93')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Delete User','1','94')  -- DeleteUserLabel
INSERT INTO Translations VALUES ('Delete Doctor','1','95')  -- DeleteDoctorLabel
INSERT INTO Translations VALUES ('Reset Password','1','96')  -- ResetPasswordLabel
INSERT INTO Translations VALUES ('Return to Settings','1','97')  -- ReturnToSettingsButton
INSERT INTO Translations VALUES ('User List','1','98')  -- UserListLabel
INSERT INTO Translations VALUES ('Manage Users','1','99')  -- ManageUsersLabel
INSERT INTO Translations VALUES ('Username:','1','100')  -- UsernameLabel
INSERT INTO Translations VALUES ('Doctor Name:','1','101')  -- DoctorsNameLabel
INSERT INTO Translations VALUES ('Admin Password:','1','102')  -- CurrentPasswordLabel
INSERT INTO Translations VALUES ('Password:','1','103')  -- PasswordLabel
INSERT INTO Translations VALUES ('Confirm Password:','1','104')  -- ConfirmPasswordLabel
INSERT INTO Translations VALUES ('Passwords don''t match!','1','105')  -- PasswordsDontMatchLabel
INSERT INTO Translations VALUES ('Passwords must contain at least eight characters and one digit.','1','106')  -- PasswordInvalidLabel
INSERT INTO Translations VALUES ('Admin','1','107')  -- AdminLabel
INSERT INTO Translations VALUES ('already exists!','1','108')  -- UsernameAlreadyExistsText
INSERT INTO Translations VALUES ('Username Exists','1','109')  -- UsernameAlreadyExistsTitle
INSERT INTO Translations VALUES ('already exists but the user is inactive.  Do you want to reactivate it ?','1','110')  -- UsernameAlreadyExistsInactive
INSERT INTO Translations VALUES ('The username:','1','111')  -- TheUsername
INSERT INTO Translations VALUES ('Re-Activate User ?','1','112')  -- ReactivateUserTitle
INSERT INTO Translations VALUES ('The physician''s name :','1','113')  -- ThePhysicianName
INSERT INTO Translations VALUES ('Do you really want to delete the user :','1','114')  -- DeleteUserMessage
INSERT INTO Translations VALUES ('Delete User','1','115')  -- DeleteUserTitle

-- *****************************************************************************************
-- Home SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Shut Down','1','116')  -- NewUserLabel
INSERT INTO Translations VALUES ('Change Tank','1','117')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Cryo Therapy','1','118')  -- EditUserLabel
INSERT INTO Translations VALUES ('Records','1','119')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Settings','1','120')  -- SettingsLabel

-- *****************************************************************************************
-- SETTINGS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('TIMERS PREFERENCES','1','121')  -- TimerSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Cooling Timer To:','1','122')  -- CoolingTimerToTextBlock
INSERT INTO Translations VALUES ('Thaw Timer To:','1','123')  -- ThawTimerToTextBlock
INSERT INTO Translations VALUES ('Ablation Timer:','1','124')  -- AblationTimerTextBlock
INSERT INTO Translations VALUES ('seconds','1','125')  -- SecondsTextBlock
INSERT INTO Translations VALUES ('Chart Type','1','126')  -- ChartTypeTextBlock
INSERT INTO Translations VALUES ('Curve Color','1','127')  -- CurveColorTextBlock

INSERT INTO Translations VALUES ('NOTIFICATIONS PREFERENCES','1','128')  -- AlertSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Low Ablation Temperature:','1','129')  -- LowAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('High Ablation Temperature:','1','130')  -- HighAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Esophagus Temperature:','1','131')  -- EsophagusTemperatureTextBlock
INSERT INTO Translations VALUES ('Diaphragm Sensor Limit:','1','132')  -- DiaphragmSensorLimitTextBlock
INSERT INTO Translations VALUES ('Diaphragm Sensor Gain:','1','133')  -- DiaphragmSensorGainTextBlock

INSERT INTO Translations VALUES ('SYSTEM SETTINGS','1','134')  -- SystemSettingsLabel
INSERT INTO Translations VALUES ('Inflate Speed','1','135')  -- InflateSpeedTextBlock
INSERT INTO Translations VALUES ('Fast','1','136')  -- FastTextBlock
INSERT INTO Translations VALUES ('Slow','1','137')  -- SlowTextBox
INSERT INTO Translations VALUES ('On','1','138')  -- OnTextBlock
INSERT INTO Translations VALUES ('Off','1','139')  -- OffTextBlock
INSERT INTO Translations VALUES ('DMS','1','140')  -- DMSTextBlock
INSERT INTO Translations VALUES ('Line','1','141')  -- LineTextBlock
INSERT INTO Translations VALUES ('Area','1','142')  -- AreaTextBlock
INSERT INTO Translations VALUES ('Refrigerant Level','1','143')  -- RefrigerantLevelTextBlock
INSERT INTO Translations VALUES ('Weight','1','144')  -- LbsTextBlock
INSERT INTO Translations VALUES ('Min','1','145')  -- MinTextBlock

-- *****************************************************************************************
-- LOGIN SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Login','1','146')  -- LoginLabel
INSERT INTO Translations VALUES ('Wrong Username or Password, please try again!','1','147')  -- WrondUsernameOrPasswordLabel
INSERT INTO Translations VALUES ('OK','1','148')  -- OkButton

-- *****************************************************************************************
-- PATIENT SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Patient information','1','149')  -- PatientInformationLabel
INSERT INTO Translations VALUES ('Patient ID','1','150')  -- PatientIdLabel
INSERT INTO Translations VALUES ('First Name','1','151')  -- FirstNameLabel
INSERT INTO Translations VALUES ('Last Name','1','152')  -- LastNameLabel
INSERT INTO Translations VALUES ('Gender','1','153')  -- GenderLabel
INSERT INTO Translations VALUES ('Male','1','154')  -- MaleLabel
INSERT INTO Translations VALUES ('Female','1','155')  -- FemaleLabel
INSERT INTO Translations VALUES ('Date Of Birth','1','156')  -- BirthDateLabel
INSERT INTO Translations VALUES ('DD','1','157')  -- DayLabel
INSERT INTO Translations VALUES ('MM','1','158')  -- MonthLabel
INSERT INTO Translations VALUES ('YYYY','1','159')  -- YearLabel
INSERT INTO Translations VALUES ('Physician','1','160')  -- PhysicianLabel

-- *****************************************************************************************
-- GENERIC SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('YES','1','161')  -- YESButton
INSERT INTO Translations VALUES ('NO','1','162')  -- NOButton
INSERT INTO Translations VALUES ('Yes','1','163')  -- YesButton
INSERT INTO Translations VALUES ('No','1','164')  -- NoButton
INSERT INTO Translations VALUES ('YES','1','165')  -- YESLabel
INSERT INTO Translations VALUES ('NO','1','166')  -- NOLabel
INSERT INTO Translations VALUES ('Yes','1','167')  -- YesLabel
INSERT INTO Translations VALUES ('No','1','168')  -- NoLabel

-- *****************************************************************************************
-- MESSAGE POPUP SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('SYSTEM MESSAGE','1','169')  -- SystemMessageLabel
INSERT INTO Translations VALUES ('WARNING MESSAGE','1','170')  -- WarningMessageLabel
INSERT INTO Translations VALUES ('SYSTEM NOTIFICATION','1','171')  -- ErrorMessageLabel

-- *****************************************************************************************
-- SETTINGS MAIN SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Date and Time','1','172')  -- DateTimeLabel
INSERT INTO Translations VALUES ('User Manual','1','173')  -- UserManualLabel
INSERT INTO Translations VALUES ('Maintenance','1','174')  -- MaintenanceLabel
INSERT INTO Translations VALUES ('Action Log','1','175')  -- ActionLogLabel
INSERT INTO Translations VALUES ('DAYS','1','176')  -- DaysLabel
INSERT INTO Translations VALUES ('HOURS','1','177')  -- HoursLabel
INSERT INTO Translations VALUES ('MINUTES','1','178')  -- MinutesLabel

-- *****************************************************************************************
-- Add new Translation here
-- *****************************************************************************************
INSERT INTO Translations VALUES ('DMS DISABLED','1','179')  -- NoPacingDetectedLabelOff
INSERT INTO Translations VALUES ('Weight','1','180')  -- WeightLabel
INSERT INTO Translations VALUES ('Height','1','181')  -- HeightLabel
INSERT INTO Translations VALUES ('Current Ablation Timer:','1','182')  -- ActualAblationTimerTextBlock
INSERT INTO Translations VALUES ('If TTI <','1','183')  -- ExpectedVeinIsolationTimeTextBlock
INSERT INTO Translations VALUES ('THEN Set Ablation Timer =','1','184')  -- NewAblationTimerTextBlock
INSERT INTO Translations VALUES ('Time To Isolation','1','185')  -- VeinIsolationLogicLabel
INSERT INTO Translations VALUES ('Update Vein Isolation Duration','1','186')  -- UpdateVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('New Duration','1','187')  -- NewVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Invalid duration!','1','188')  -- InvalidDurationLabel
INSERT INTO Translations VALUES ('Ablation Timers','1','189')  -- AblationTimersLabel
INSERT INTO Translations VALUES ('Fixed Timer:','1','190')  -- FixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI Fixed Timer:','1','191')  -- TTIFixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI + Duration Timer:','1','192')  -- TTIDurationTimerTextBlock
INSERT INTO Translations VALUES ('Ablation Timer:  TTI + ','1','193')  -- AblationTimerTTIPlusTextBlock

INSERT INTO Translations VALUES ('Temperature','1','194')  -- CryoBalloonTemperatureLabel
INSERT INTO Translations VALUES ('Please select an ablation site','1','195')  -- SelectAblationSiteLabel
INSERT INTO Translations VALUES ('Exit Playback','1','196')  -- ExitPlaybackButton
INSERT INTO Translations VALUES ('Playback Mode','1','197')  -- PlaybackModeLabel
INSERT INTO Translations VALUES ('Complete Procedure','1','198')  -- CompleteProcedureButton
INSERT INTO Translations VALUES ('Return to Procedure','1','199')  -- ReturnToProcedureButton
INSERT INTO Translations VALUES ('End Procedure','1','200')  -- EndProcedureButton
INSERT INTO Translations VALUES ('Playback','1','201')  -- PlaybackButton
INSERT INTO Translations VALUES ('Case Date','1','202')  -- CaseDateLabel
INSERT INTO Translations VALUES ('SAVE TO USB DRIVE','1','203')  -- SaveToUSBDriveLabel
INSERT INTO Translations VALUES ('Volume Label:','1','204')  -- VolumeLabel
INSERT INTO Translations VALUES ('Name:','1','205')  -- NameLabel
INSERT INTO Translations VALUES ('Drive Format:','1','206')  -- DriveFormatLabel
INSERT INTO Translations VALUES ('Free Space (Bytes):','1','207')  -- FreeSpaceLabel
INSERT INTO Translations VALUES ('Total Size (Bytes):','1','208')  -- TotalSizeLabel
INSERT INTO Translations VALUES ('File Name:','1','209')  -- FileNameLabel
INSERT INTO Translations VALUES ('File Type:','1','210')  -- FileTypeLabel
INSERT INTO Translations VALUES ('Back To Treatment Record','1','211')  -- BackToTreatmentRecordButton
INSERT INTO Translations VALUES ('User','1','212')  -- UserLabel
INSERT INTO Translations VALUES ('Home','1','213')  -- HomeLabel
INSERT INTO Translations VALUES ('Access Type','1','214')  -- AccessTypeLabel
INSERT INTO Translations VALUES ('Then','1','215')  -- ThenTextBlock
INSERT INTO Translations VALUES ('Else','1','216')  -- ElseTextBlock
INSERT INTO Translations VALUES ('Select a Date…','1','217')  -- SelectADateLabel
INSERT INTO Translations VALUES ('Qualified Person Only','1','218')  -- QualifiedPersonLabel
INSERT INTO Translations VALUES ('Message','1','219')  -- MessageLabel
INSERT INTO Translations VALUES ('Reset System','1','220')  -- ResetSystemButton
INSERT INTO Translations VALUES ('Close','1','221')  -- CloseButton
INSERT INTO Translations VALUES ('Action Required','1','222')  -- ActionRequiredLabel
INSERT INTO Translations VALUES ('Update Duration','1','223')  -- UpdateDurationLabel
INSERT INTO Translations VALUES ('New Duration','1','224')  -- NewDurationLabel
INSERT INTO Translations VALUES ('Any change to the ablation site will be saved','1','225')  -- UpdateAblationSiteWarningLabel
INSERT INTO Translations VALUES ('Multiple files selected.  Will be saved in :','1','226') -- MultipleFilesSelectedLabel
INSERT INTO Translations VALUES ('Error','1','227') -- ErrorLabel
INSERT INTO Translations VALUES ('Invalid! ','1','228') -- InvalidDMSThresholdLabel
INSERT INTO Translations VALUES ('Min: ','1','229') -- MinLabel
INSERT INTO Translations VALUES ('Max: ','1','230') -- MaxLabel
INSERT INTO Translations VALUES ('Audio Alert','1','231') -- AudioAlertTextBlock

-- *****************************************************************************************
-- ENGLISH TRANSLATION - END
-- *****************************************************************************************



-- *****************************************************************************************
-- GERMAN TRANSLATION - BEGIN
-- *****************************************************************************************
-- CRYOTHERAPY SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Start','2','1')  -- StartButton
INSERT INTO Translations VALUES ('Stopp','2','2')  -- StopButton
INSERT INTO Translations VALUES ('VAKUUM AN','2','3')  -- ConnectButton
INSERT INTO Translations VALUES ('VAKUUM AUS','2','4')  -- DisconnectButton
INSERT INTO Translations VALUES ('Vene isoliert','2','5')  -- VeinIsolationTextBlock
INSERT INTO Translations VALUES ('Einstellungen','2','6')  -- NotificationTextBlock
INSERT INTO Translations VALUES ('Anmerkungen','2','7')  -- NotesTextBlock
INSERT INTO Translations VALUES ('Luft ablassen bei Auftauen','2','8')  -- DeflateAtThawTextBlock
INSERT INTO Translations VALUES ('STATUS:','2','9')  -- STATUSLabel
INSERT INTO Translations VALUES ('LEERLAUF','2','10')  -- IDLELabel
INSERT INTO Translations VALUES ('BEREIT','2','11') -- READYLabel
INSERT INTO Translations VALUES ('AUFBLASEN','2','12')  -- INFLATIONLabel
INSERT INTO Translations VALUES ('ABLATION','2','13')  -- ABLATIONLabel
INSERT INTO Translations VALUES ('AUFTAUEN','2','14')  -- THAWINGLabel
INSERT INTO Translations VALUES ('ABLATION ZUSAMMENFASSUNG','2','15')  -- ABLATIONSUMMARYLabel
INSERT INTO Translations VALUES ('Ablationsstelle','2','16')  -- AblationSiteLabel
INSERT INTO Translations VALUES ('Ablationen','2','17')  -- AblationsLabel
INSERT INTO Translations VALUES ('Dauer (s)','2','18')  -- DurationInSecLabel
INSERT INTO Translations VALUES ('RSPV','2','19')  -- RSPVLabel
INSERT INTO Translations VALUES ('RIPV','2','20')  -- RIPVLabel
INSERT INTO Translations VALUES ('LSPV','2','21')  -- LSPVLabel
INSERT INTO Translations VALUES ('LIPV','2','22')  -- LIPVLabel
INSERT INTO Translations VALUES ('SONSTIGE','2','23')  -- OTHERLabel
INSERT INTO Translations VALUES ('Gesamt:','2','24')  -- TotalLabel
INSERT INTO Translations VALUES ('Verstrichene Zeit:','2','25')  -- ElapsedTimeLabel
INSERT INTO Translations VALUES ('min','2','26')  -- minLabel
INSERT INTO Translations VALUES ('Aktuelle Zeit:','2','27')  -- CurrentTimeLabel
INSERT INTO Translations VALUES ('TEMPERATUR','2','28')  -- TEMPERATURELabel
INSERT INTO Translations VALUES ('ABLATIONSZEIT','2','29')  -- AblationTimeLabel
INSERT INTO Translations VALUES ('TIMER & TEMPO','2','30')  -- TIMERSAndRATESLabel
INSERT INTO Translations VALUES ('ALARMSIGNALE & ANZEIGEN','2','31')  -- ALERTSAndINDICATORSLabel
INSERT INTO Translations VALUES ('SYSTEMINFO','2','32')  -- SystemINFOLabel
INSERT INTO Translations VALUES ('TEMPERATUR TEMPO','2','33')  -- TEMPERATUREAndRATELabel
INSERT INTO Translations VALUES ('MINDESTTEMPERATUR','2','34')  -- MinimumTEMPERATURELabel
INSERT INTO Translations VALUES ('ABKÜHLZEIT BIS','2','35')  -- CoolingTimeToLabel
INSERT INTO Translations VALUES ('ZEIT BIS EFFEKT','2','36')  -- TimeToEffectLabel
INSERT INTO Translations VALUES ('AUFTAUZEIT BIS','2','37')  -- ThawTimeToLabel
INSERT INTO Translations VALUES ('DIAPHRAGMA','2','38')  -- DiaphragmLabel
INSERT INTO Translations VALUES ('BEWEGUNG','2','39')  -- MovementLabel
INSERT INTO Translations VALUES ('Zoom:','2','40')  -- ZoomLabel
INSERT INTO Translations VALUES ('KEINE STIMULATION ENTDECKT','2','41')  -- NoPacingDetectedLabel
INSERT INTO Translations VALUES ('Dient nur als Referenz. Verlassen Sie sich nie auf diese Anzeigen alleine','2','42')  -- ForReferenceOnlyNeverRelySolelyOnTheseIndicatorsLabel
INSERT INTO Translations VALUES ('ÖSOPHAGUS','2','43')  -- ESOPHAGUSLabel
INSERT INTO Translations VALUES ('TEMPERATUR','2','44')  -- TEMPERATURELabel1
INSERT INTO Translations VALUES ('10 °C bis 40 °C','2','45')  -- TemperatureRangeLabel
INSERT INTO Translations VALUES ('DURCHFLUSS:','2','46')  -- FlowLabel
INSERT INTO Translations VALUES ('DRUCK:','2','47')  -- PRESSURELabel
INSERT INTO Translations VALUES ('BALLON:','2','48')  -- BalloonLabel
INSERT INTO Translations VALUES ('Ablationsdauer','2','49')  -- TimerLabel
INSERT INTO Translations VALUES ('s','2','50')  -- SecLabel
INSERT INTO Translations VALUES ('Behandlung:','2','51')  -- TreatmentLabel
INSERT INTO Translations VALUES ('von','2','52')  -- TreatmentNumberOf
INSERT INTO Translations VALUES ('Bitte warten, System wird initialisiert...','2','53')  -- WaitSystemIsInitializing

-- *****************************************************************************************
-- TREATMENT RECORDS SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('BEHANDLUNGSAUFZEICHNUNGEN','2','54')  -- TreatmentRecordTitleLabel
INSERT INTO Translations VALUES ('PATIENTENANGABEN','2','55')  -- PatientInfoLabel
INSERT INTO Translations VALUES ('Patient','2','56')  -- PatientNameLabel
INSERT INTO Translations VALUES ('Geschlecht','2','57')  -- PatientGenderLabel
INSERT INTO Translations VALUES ('Geburtsdatum','2','58')  -- PatientBirthDateLabel
INSERT INTO Translations VALUES ('ID-Nummer','2','59')  -- PatientIdNumberLabel
INSERT INTO Translations VALUES ('VERFAHRENSANGABEN','2','60')  -- ProcedureInfoLabel
INSERT INTO Translations VALUES ('Arzt','2','61')  -- PhysicianNameLabel
INSERT INTO Translations VALUES ('Verwendeter Katheter','2','62')  -- CatheterUsedLabel
INSERT INTO Translations VALUES ('Verfahrensdatum','2','63')  -- ProcedureDateLabel
INSERT INTO Translations VALUES ('ÖSOPHAGUS-TEMPERATUR','2','64')  -- EsophagusTemperatureLabel
INSERT INTO Translations VALUES ('DIAPHRAGMA-BEWEGUNG','2','65')  -- DiaphragmMovementLabel
INSERT INTO Translations VALUES ('BALLONDRUCK','2','66')  -- BalloonPressureLabel
INSERT INTO Translations VALUES ('VERFAHRENSAUFZEICHNUNGEN','2','67')  -- ProcedureRecordsLabel
INSERT INTO Translations VALUES ('Auf USB speichern','2','68')  -- ExportProcedureButton
INSERT INTO Translations VALUES ('Engineering-Daten werden gespeichert...','2','69')  -- SaveEngineeringDataInProgressLabel

-- *****************************************************************************************
-- SUMMARY REPORT SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('ZUSAMMENFASSENDER BERICHT','2','70')  -- SummaryReportLabel
INSERT INTO Translations VALUES ('BEHANDLUNGSANGABEN','2','71')  -- TreatmentInfoLabel
INSERT INTO Translations VALUES ('Diagnose','2','72')  -- DiagnosisLabel
INSERT INTO Translations VALUES ('Ausgang','2','73')  -- OutcomeLabel

-- *****************************************************************************************
-- CHANGE TANK SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('TANK-AUSTAUSCH','2','74')  -- TankReplacementLabel
INSERT INTO Translations VALUES ('Tank schließen, dann auf Weiter drücken','2','75')  -- CloseTankLabel
INSERT INTO Translations VALUES ('Bitte warten, während die Konsole die Leitung aus Sicherheitsgründen durchspült','2','76')  -- WaitLabel
INSERT INTO Translations VALUES ('Tank austauschen','2','77')  -- ReplaceTankLabel
INSERT INTO Translations VALUES ('Tank öffnen','2','78')  -- OpenTankLabel
INSERT INTO Translations VALUES ('Für ein sicheres Austauschen des Tanks bitte die Anweisungen befolgen','2','79')  -- FollowInstructionsLabel
INSERT INTO Translations VALUES ('Tank-Austausch erfolgreich!','2','80')  -- ChangeTankSuccessLabel
INSERT INTO Translations VALUES ('Weiter','2','81')  -- NextButton
INSERT INTO Translations VALUES ('Beenden','2','82')  -- FinishButton
INSERT INTO Translations VALUES ('Abbrechen','2','83')  -- CancelButton
INSERT INTO Translations VALUES ('Austauschdatum :','2','84')  -- ChangeDateLabel
INSERT INTO Translations VALUES ('Gewicht bei Austausch :','2','85')  -- WeightAtChangeLabel
INSERT INTO Translations VALUES ('Aktuelles Gewicht :','2','86')  -- CurrentWeightLabel
INSERT INTO Translations VALUES ('Art des Tanks für Austausch wählen :','2','87')  -- SelectTankReplacementTypeLabel
INSERT INTO Translations VALUES ('10 Pfund','2','88')  -- TenPoundsLabel
INSERT INTO Translations VALUES ('15 Pfund','2','89')  -- FifteenPoundsLabel

-- *****************************************************************************************
-- MANAGE USERS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Neuer Benutzer','2','90')  -- NewUserLabel
INSERT INTO Translations VALUES ('Neuer Arzt','2','91')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Benutzer bearbeiten','2','92')  -- EditUserLabel
INSERT INTO Translations VALUES ('Arzt bearbeiten','2','93')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Benutzer löschen','2','94')  -- DeleteUserLabel
INSERT INTO Translations VALUES ('Arzt löschen','2','95')  -- DeleteDoctorLabel
INSERT INTO Translations VALUES ('Passwort zurücksetzen','2','96')  -- ResetPasswordLabel
INSERT INTO Translations VALUES ('Zurück zu Einstellungen','2','97')  -- ReturnToSettingsButton
INSERT INTO Translations VALUES ('Benutzerliste','2','98')  -- UserListLabel
INSERT INTO Translations VALUES ('Benutzer verwalten','2','99')  -- ManageUsersLabel
INSERT INTO Translations VALUES ('Benutzername:','2','100')  -- UsernameLabel
INSERT INTO Translations VALUES ('Arztname:','2','101')  -- DoctorsNameLabel
INSERT INTO Translations VALUES ('Aktuelles Passwort:','2','102')  -- CurrentPasswordLabel
INSERT INTO Translations VALUES ('Passwort:','2','103')  -- PasswordLabel
INSERT INTO Translations VALUES ('Passwort bestätigen:','2','104')  -- ConfirmPasswordLabel
INSERT INTO Translations VALUES ('Passwörter stimmen nicht überein!','2','105')  -- PasswordsDontMatchLabel
INSERT INTO Translations VALUES ('Passwörter müssen mindestens acht Zeichen und eine Zahl enthalten.','2','106')  -- PasswordInvalidLabel
INSERT INTO Translations VALUES ('Admin','2','107')  -- AdminLabel
INSERT INTO Translations VALUES ('existiert bereits!','2','108')  -- UsernameAlreadyExistsText
INSERT INTO Translations VALUES ('Benutzername existiert','2','109')  -- UsernameAlreadyExistsTitle
INSERT INTO Translations VALUES ('existiert bereits, doch der Benutzer ist inaktiv.  Möchten Sie ihn reaktivieren ?','2','110')  -- UsernameAlreadyExistsInactive
INSERT INTO Translations VALUES ('Der Benutzername:','2','111')  -- TheUsername
INSERT INTO Translations VALUES ('Benutzer reaktivieren ?','2','112')  -- ReactivateUserTitle
INSERT INTO Translations VALUES ('Der Arztname :','2','113')  -- ThePhysicianName
INSERT INTO Translations VALUES ('Möchten Sie den Benutzer wirklich löschen :','2','114')  -- DeleteUserMessage
INSERT INTO Translations VALUES ('Benutzer löschen','2','115')  -- DeleteUserTitle

-- *****************************************************************************************
-- Home SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Abschalten','2','116')  -- NewUserLabel
INSERT INTO Translations VALUES ('Tank austauschen','2','117')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Kryotherapie','2','118')  -- EditUserLabel
INSERT INTO Translations VALUES ('Aufzeichnungen','2','119')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Settings','2','120')  -- DeleteUserLabel

-- *****************************************************************************************
-- SETTINGS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('TIMER EINSTELLUNGEN VOREINSTELLUNGEN','2','121')  -- TimerSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Kühlender Timer zu:','2','122')  -- CoolingTimerToTextBlock
INSERT INTO Translations VALUES ('Auftau-Timer an:','2','123')  -- ThawTimerToTextBlock
INSERT INTO Translations VALUES ('Ablationstimer:','2','124')  -- AblationTimerTextBlock
INSERT INTO Translations VALUES ('sekunden','2','125')  -- SecondsTextBlock
INSERT INTO Translations VALUES ('Diagramm Typ','2','126')  -- ChartTypeTextBlock
INSERT INTO Translations VALUES ('Kurvenfarbe','2','127')  -- CurveColorTextBlock

INSERT INTO Translations VALUES ('ALARMEINSTELLUNGEN VOREINSTELLUNGEN','2','128')  -- AlertSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Niedrige Ablationstemperatur:','2','129')  -- LowAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Hohe Ablationstemperatur:','2','130')  -- HighAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Ösophagustemperatur:','2','131')  -- EsophagusTemperatureTextBlock
INSERT INTO Translations VALUES ('Begrenzung des Membransensors:','2','132')  -- DiaphragmSensorLimitTextBlock
INSERT INTO Translations VALUES ('Membransensor-Verstärkung:','2','133')  -- DiaphragmSensorGainTextBlock

INSERT INTO Translations VALUES ('SYSTEMEINSTELLUNGEN','2','134')  -- SystemSettingsLabel
INSERT INTO Translations VALUES ('Geschwindigkeit aufpumpen','2','135')  -- InflateSpeedTextBlock
INSERT INTO Translations VALUES ('Schnell','2','136')  -- FastTextBlock
INSERT INTO Translations VALUES ('Langsam','2','137')  -- SlowTextBox
INSERT INTO Translations VALUES ('Auf','2','138')  -- OnTextBlock
INSERT INTO Translations VALUES ('Aus','2','139')  -- OffTextBlock
INSERT INTO Translations VALUES ('DMS','2','140')  -- DMSTextBlock
INSERT INTO Translations VALUES ('Linie','2','141')  -- LineTextBlock
INSERT INTO Translations VALUES ('Bereich','2','142')  -- AreaTextBlock
INSERT INTO Translations VALUES ('Kältemittel-Niveau','2','143')  -- RefrigerantLevelTextBlock
INSERT INTO Translations VALUES ('Weight','2','144')  -- LbsTextBlock
INSERT INTO Translations VALUES ('Min','2','145')  -- MinTextBlock

-- *****************************************************************************************
-- LOGIN SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Anmeldung','2','146')  -- LoginLabel
INSERT INTO Translations VALUES ('Falscher Benutzername oder falsches Passwort. Bitte versuche es erneut!','2','147')  -- WrondUsernameOrPasswordLabel
INSERT INTO Translations VALUES ('OK','2','148')  -- OkButton

-- *****************************************************************************************
-- PATIENT SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Informationen zum Patienten','2','149')  -- PatientInformationLabel
INSERT INTO Translations VALUES ('Patienten ID','2','150')  -- PatientIdLabel
INSERT INTO Translations VALUES ('Vorname','2','151')  -- FirstNameLabel
INSERT INTO Translations VALUES ('Familienname, Nachname','2','152')  -- LastNameLabel
INSERT INTO Translations VALUES ('Geschlecht','2','153')  -- GenderLabel
INSERT INTO Translations VALUES ('Männlich','2','154')  -- MaleLabel
INSERT INTO Translations VALUES ('Weiblich','2','155')  -- FemaleLabel
INSERT INTO Translations VALUES ('Date Of BirthGeburtsdatum','2','156')  -- BirthDateLabel
INSERT INTO Translations VALUES ('DD','2','157')  -- DayLabel
INSERT INTO Translations VALUES ('MM','2','158')  -- MonthLabel
INSERT INTO Translations VALUES ('YYYY','2','159')  -- YearLabel
INSERT INTO Translations VALUES ('Arzt','2','160')  -- PhysicianLabel

-- *****************************************************************************************
-- GENERIC SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('JA','2','161')  -- YESButton
INSERT INTO Translations VALUES ('NEIN','2','162')  -- NOButton
INSERT INTO Translations VALUES ('Ja','2','163')  -- YesButton
INSERT INTO Translations VALUES ('Nein','2','164')  -- NoButton
INSERT INTO Translations VALUES ('JA','2','165')  -- YESLabel
INSERT INTO Translations VALUES ('NEIN','2','166')  -- NOLabel
INSERT INTO Translations VALUES ('Ja','2','167')  -- YesLabel
INSERT INTO Translations VALUES ('Nein','2','168')  -- NoLabel

-- *****************************************************************************************
-- MESSAGE POPUP SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('SYSTEMNACHRICHT','2','169')  -- SystemMessageLabel
INSERT INTO Translations VALUES ('WARNMELDUNG','2','170')  -- WarningMessageLabel
INSERT INTO Translations VALUES ('FEHLERMELDUNG','2','171')  -- ErrorMessageLabel

-- *****************************************************************************************
-- SETTINGS MAIN SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Zeit und Datum','2','172')  -- DateTimeLabel
INSERT INTO Translations VALUES ('Benutzerhandbuch','2','173')  -- UserManualLabel
INSERT INTO Translations VALUES ('Instandhaltung','2','174')  -- MaintenanceLabel
INSERT INTO Translations VALUES ('Aktionsprotokoll','2','175')  -- ActionLogLabel
INSERT INTO Translations VALUES ('TAGE','2','176')  -- DaysLabel
INSERT INTO Translations VALUES ('STD','2','177')  -- HoursLabel
INSERT INTO Translations VALUES ('Protokoll','2','178')  -- MinutesLabel

-- *****************************************************************************************
-- Add new Translation here
-- *****************************************************************************************
INSERT INTO Translations VALUES ('THE DMS IS OFF','2','179')  -- NoPacingDetectedLabelOff
INSERT INTO Translations VALUES ('Gewicht','2','180')  -- WeightLabel
INSERT INTO Translations VALUES ('Höhe','2','181')  -- HeightLabel
INSERT INTO Translations VALUES ('Current Ablation Timer:','2','182')  -- ActualAblationTimerTextBlock
INSERT INTO Translations VALUES ('IF Vein Isolation Time >','2','183')  -- ExpectedVeinIsolationTimeTextBlock
INSERT INTO Translations VALUES ('THEN Set Ablation Timer =','2','184')  -- NewAblationTimerTextBlock
INSERT INTO Translations VALUES ('VEIN ISOLATION LOGIC','2','185')  -- VeinIsolationLogicLabel
INSERT INTO Translations VALUES ('Update der Venenisolationsdauer','2','186')  -- UpdateVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Neue Dauer','2','187')  -- NewVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Ungültige Dauer!','2','188')  -- InvalidDurationLabel
INSERT INTO Translations VALUES ('Ablation Timers','2','189')  -- AblationTimersLabel
INSERT INTO Translations VALUES ('Feste Timer:','2','190')  -- FixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI Feste Timer:','2','191')  -- TTIFixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI + Dauer Timer:','2','192')  -- TTIDurationTimerTextBlock
INSERT INTO Translations VALUES ('Ablation Timer:  TTI + ','2','193')  -- AblationTimerTTIPlusTextBlock

INSERT INTO Translations VALUES ('Temperatur','2','194')  -- CryoBalloonTemperatureLabel
INSERT INTO Translations VALUES ('Bitte wählen Sie eine Ablationsstelle','2','195')  -- SelectAblationSiteLabel
INSERT INTO Translations VALUES ('Beenden Sie die Wiedergabe','2','196')  -- ExitPlaybackButton
INSERT INTO Translations VALUES ('Wiedergabemodus','2','197')  -- PlaybackModeLabel
INSERT INTO Translations VALUES ('Vollständiges Verfahren','2','198')  -- CompleteProcedureButton
INSERT INTO Translations VALUES ('Zurück zum Verfahren','2','199')  -- ReturnToProcedureButton
INSERT INTO Translations VALUES ('Ende der Prozedur','2','200')  -- EndProcedureButton
INSERT INTO Translations VALUES ('Wiedergabe','2','201')  -- PlaybackButton
INSERT INTO Translations VALUES ('Falldatum','2','202')  -- CaseDateLabel
INSERT INTO Translations VALUES ('SPEICHERN SIE AUF USB-LAUFWERK','2','203')  -- SaveToUSBDriveLabel
INSERT INTO Translations VALUES ('Volumen Label:','2','204')  -- VolumeLabel
INSERT INTO Translations VALUES ('Name:','2','205')  -- NameLabel
INSERT INTO Translations VALUES ('Laufwerksformat:','2','206')  -- DriveFormatLabel
INSERT INTO Translations VALUES ('Freier Speicherplatz (Bytes):','2','207')  -- FreeSpaceLabel
INSERT INTO Translations VALUES ('Gesamtgröße (Bytes):','2','208')  -- TotalSizeLabel
INSERT INTO Translations VALUES ('Dateiname:','2','209')  -- FileNameLabel
INSERT INTO Translations VALUES ('Dateityp:','2','210')  -- FileTypeLabel
INSERT INTO Translations VALUES ('Zurück zum Behandlungsprotokoll','2','211')  -- BackToTreatmentRecordButton
INSERT INTO Translations VALUES ('Benutzer','2','212')  -- UserLabel
INSERT INTO Translations VALUES ('Zuhause','2','213')  -- HomeLabel
INSERT INTO Translations VALUES ('Zugangsart','2','214')  -- AccessTypeLabel
INSERT INTO Translations VALUES ('Dann','2','215')  -- ThenTextBlock
INSERT INTO Translations VALUES ('Sonst','2','216')  -- ElseTextBlock
INSERT INTO Translations VALUES ('Wählen Sie ein Datum ...','2','217')  -- SelectADateLabel
INSERT INTO Translations VALUES ('Nur qualifizierte Person','2','218')  -- QualifiedPersonLabel
INSERT INTO Translations VALUES ('Botschaft','2','219')  -- MessageLabel
INSERT INTO Translations VALUES ('System zurücksetzen','2','220')  -- ResetSystemButton
INSERT INTO Translations VALUES ('Schließen','2','221')  -- CloseButton
INSERT INTO Translations VALUES ('Handlung erforderlich','2','222')  -- ActionRequiredLabel
INSERT INTO Translations VALUES ('Aktualisierungsdauer','2','223')  -- UpdateDurationLabel
INSERT INTO Translations VALUES ('Neue Dauer','2','224')  -- NewDurationLabel
INSERT INTO Translations VALUES ('Änderungen an der Ablationsstelle werden gespeichert','2','225')  -- UpdateAblationSiteWarningLabel
INSERT INTO Translations VALUES ('Mehrere Dateien ausgewählt Wird gespeichert in: ','2','226') -- MultipleFilesSelectedLabel
INSERT INTO Translations VALUES ('Error','2','227') -- ErrorLabel
INSERT INTO Translations VALUES ('Ungültig! ','2','228') -- InvalidDMSThresholdLabel
INSERT INTO Translations VALUES ('Min: ','2','229') -- MinLabel
INSERT INTO Translations VALUES ('Max: ','2','230') -- MaxLabel
INSERT INTO Translations VALUES ('Audiowarnung','2','231') -- AudioAlertTextBlock
-- *****************************************************************************************
-- GERMAN TRANSLATION - END
-- *****************************************************************************************



-- *****************************************************************************************
-- FRENCH TRANSLATION - BEGIN
-- *****************************************************************************************
-- CRYOTHERAPY SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Démarrer','3','1')  -- StartButton
INSERT INTO Translations VALUES ('Arrêter','3','2')  -- StopButton
INSERT INTO Translations VALUES ('Connecter','3','3')  -- ConnectButton
INSERT INTO Translations VALUES ('Déconnecter','3','4')  -- DisconnectButton
INSERT INTO Translations VALUES ('Veine Isolée','3','5')  -- VeinIsolationTextBlock
INSERT INTO Translations VALUES ('Paramètres','3','6')  -- NotificationTextBlock
INSERT INTO Translations VALUES ('Remarques','3','7')  -- NotesTextBlock
INSERT INTO Translations VALUES ('Dégonfler au dégel','3','8')  -- DeflateAtThawTextBlock
INSERT INTO Translations VALUES ('Statut:','3','9')  -- STATUSLabel
INSERT INTO Translations VALUES ('Inactif','3','10')  -- IDLELabel
INSERT INTO Translations VALUES ('Prêt','3','11')  -- READYLabel
INSERT INTO Translations VALUES ('Inflation','3','12')  -- INFLATIONLabel
INSERT INTO Translations VALUES ('Ablation','3','13')  -- ABLATIONLabel
INSERT INTO Translations VALUES ('Dégel','3','14')  -- THAWINGLabel
INSERT INTO Translations VALUES ('Résumé de l’ablation','3','15')  -- ABLATIONSUMMARYLabel
INSERT INTO Translations VALUES ('Site de l’ablation','3','16')  -- AblationSiteLabel
INSERT INTO Translations VALUES ('Ablations','3','17')  -- AblationsLabel
INSERT INTO Translations VALUES ('Durée (sec)','3','18')  -- DurationInSecLabel
INSERT INTO Translations VALUES ('VPSD','3','19')  -- RSPVLabel
INSERT INTO Translations VALUES ('VPID','3','20')  -- RIPVLabel
INSERT INTO Translations VALUES ('VPSG','3','21')  -- LSPVLabel
INSERT INTO Translations VALUES ('VPIG','3','22')  -- LIPVLabel
INSERT INTO Translations VALUES ('Autre','3','23')  -- OTHERLabel
INSERT INTO Translations VALUES ('Total:','3','24')  -- TotalLabel
INSERT INTO Translations VALUES ('Temps écoulé','3','25')  -- ElapsedTimeLabel
INSERT INTO Translations VALUES ('min','3','26')  -- minLabel
INSERT INTO Translations VALUES ('Temps actuel','3','27')  -- CurrentTimeLabel
INSERT INTO Translations VALUES ('Température','3','28')  -- TEMPERATURELabel
INSERT INTO Translations VALUES ('Temps d’ablation','3','29')  -- AblationTimeLabel
INSERT INTO Translations VALUES ('Minuteurs & Taux','3','30')  -- TIMERSAndRATESLabel
INSERT INTO Translations VALUES ('ALERTES & INDICATEURS','3','31')  -- ALERTSAndINDICATORSLabel
INSERT INTO Translations VALUES ('Infos système','3','32')  -- SystemINFOLabel
INSERT INTO Translations VALUES ('Taux de Température','3','33')  -- TEMPERATUREAndRATELabel
INSERT INTO Translations VALUES ('Température minimale','3','34')  -- MinimumTEMPERATURELabel
INSERT INTO Translations VALUES ('Temps de refroidissement à','3','35')  -- CoolingTimeToLabel
INSERT INTO Translations VALUES ('Le temps pour l’effet','3','36')  -- TimeToEffectLabel
INSERT INTO Translations VALUES ('Temps de dégel à','3','37')  -- ThawTimeToLabel
INSERT INTO Translations VALUES ('Diaphragme','3','38')  -- DiaphragmLabel
INSERT INTO Translations VALUES ('Mouvement','3','39')  -- MovementLabel
INSERT INTO Translations VALUES ('Zoom : ','3','40')  -- ZoomLabel
INSERT INTO Translations VALUES ('Pas de rythme mesuré','3','41')  -- NoPacingDetectedLabel
INSERT INTO Translations VALUES ('Uniquement pour référence. Ne vous baser pas sur ces indicateurs ','3','42')  -- ForReferenceOnlyNeverRelySolelyOnTheseIndicatorsLabel
INSERT INTO Translations VALUES ('Oesophage','3','43')  -- ESOPHAGUSLabel
INSERT INTO Translations VALUES ('Température','3','44')  -- TEMPERATURELabel1
INSERT INTO Translations VALUES ('10°C to 40°C','3','45')  -- TemperatureRangeLabel
INSERT INTO Translations VALUES ('Flux :','3','46')  -- FlowLabel
INSERT INTO Translations VALUES ('Pression : ','3','47')  -- PRESSURELabel
INSERT INTO Translations VALUES ('Ballon :','3','48')  -- BalloonLabel
INSERT INTO Translations VALUES ('Durée Ablation','3','49')  -- TimerLabel
INSERT INTO Translations VALUES ('sec','3','50')  -- SecLabel
INSERT INTO Translations VALUES ('Traitement :','3','51')  -- TreatmentLabel
INSERT INTO Translations VALUES ('De','3','52')  -- TreatmentNumberOf
INSERT INTO Translations VALUES ('Système est en cours d initialisation…','3','53')  -- WaitSystemIsInitializing

-- *****************************************************************************************
-- TREATMENT RECORDS SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('DONNÉES DU TRAITEMENT','3','54')  -- TreatmentRecordTitleLabel
INSERT INTO Translations VALUES ('INFORMATION PATIENT','3','55')  -- PatientInfoLabel
INSERT INTO Translations VALUES ('Patient','3','56')  -- PatientNameLabel
INSERT INTO Translations VALUES ('Genre','3','57')  -- PatientGenderLabel
INSERT INTO Translations VALUES ('Date de Naissance','3','58')  -- PatientBirthDateLabel
INSERT INTO Translations VALUES ('Identifiant','3','59')  -- PatientIdNumberLabel
INSERT INTO Translations VALUES ('INFORMATION DE LA PROCÉDURE','3','60')  -- ProcedureInfoLabel
INSERT INTO Translations VALUES ('MÉDECIN','3','61')  -- PhysicianNameLabel
INSERT INTO Translations VALUES ('Catheter Utilisé','3','62')  -- CatheterUsedLabel
INSERT INTO Translations VALUES ('Date Procédure','3','63')  -- ProcedureDateLabel
INSERT INTO Translations VALUES ('TEMPÉRATURE OESOPHAGE','3','64')  -- EsophagusTemperatureLabel
INSERT INTO Translations VALUES ('MOUVEMENT DIAPHRAGME','3','65')  -- DiaphragmMovementLabel
INSERT INTO Translations VALUES ('PRESSION BALLON','3','66')  -- BalloonPressureLabel
INSERT INTO Translations VALUES ('PROCÉDURES ENREGISTRÉES','3','67')  -- ProcedureRecordsLabel
INSERT INTO Translations VALUES ('Exporter vers USB','3','68')  -- ExportProcedureButton
INSERT INTO Translations VALUES ('Sauvegarde des données en cours','3','69')  -- SaveEngineeringDataInProgressLabel

-- *****************************************************************************************
-- SUMMARY REPORT SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('RAPPORT RÉCAPITULATIF','3','70')  -- SummaryReportLabel
INSERT INTO Translations VALUES ('INFORMATION DU TRAITEMENT','3','71')  -- TreatmentInfoLabel
INSERT INTO Translations VALUES ('Diagnostique','3','72')  -- DiagnosisLabel
INSERT INTO Translations VALUES ('Résultat','3','73')  -- OutcomeLabel

-- *****************************************************************************************
-- CHANGE TANK SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('REMPLACEMENT DU RÉSERVOIR','3','74')  -- TankReplacementLabel
INSERT INTO Translations VALUES ('Ouvrez le réservoir et appuyez sur Suivant','3','75')  -- CloseTankLabel
INSERT INTO Translations VALUES ('Pour votre sécurité, veuillez attendre pendant la purge du système','3','76')  -- WaitLabel
INSERT INTO Translations VALUES ('Remplacer le réservoir','3','77')  -- ReplaceTankLabel
INSERT INTO Translations VALUES ('Ouvrez le réservoir','3','78')  -- OpenTankLabel
INSERT INTO Translations VALUES ('Veuillez suivre les instructions pour un remplacement sécuritaire','3','79')  -- FollowInstructionsLabel
INSERT INTO Translations VALUES ('Réservoir remplacé avec succès!','3','80')  -- ChangeTankSuccessLabel
INSERT INTO Translations VALUES ('Suivant','3','81')  -- NextButton
INSERT INTO Translations VALUES ('Terminer','3','82')  -- FinishButton
INSERT INTO Translations VALUES ('Annuler','3','83')  -- CancelButton
INSERT INTO Translations VALUES ('Date de changement :','3','84')  -- ChangeDateLabel
INSERT INTO Translations VALUES ('Poids au changement :','3','85')  -- WeightAtChangeLabel
INSERT INTO Translations VALUES ('Poids courant :','3','86')  -- CurrentWeightLabel
INSERT INTO Translations VALUES ('Sélectionnez le type du réservoir de remplacement :','3','87')  -- SelectTankReplacementTypeLabel
INSERT INTO Translations VALUES ('10 livres','3','88')  -- TenPoundsLabel
INSERT INTO Translations VALUES ('15 livres ','3','89')  -- FifteenPoundsLabel

-- *****************************************************************************************
-- MANAGE USERS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Ajouter Usager','3','90')  -- NewUserLabel
INSERT INTO Translations VALUES ('Ajouter Docteur','3','91')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Modifier Usager','3','92')  -- EditUserLabel
INSERT INTO Translations VALUES ('Modifier Docteur','3','93')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Supprimer Usager','3','94')  -- DeleteUserLabel
INSERT INTO Translations VALUES ('Supprimer Docteur','3','95')  -- DeleteDoctorLabel
INSERT INTO Translations VALUES ('Réinitialiser Mot de Passe','3','96')  -- ResetPasswordLabel
INSERT INTO Translations VALUES ('Retour Paramètres','3','97')  -- ReturnToSettingsButton
INSERT INTO Translations VALUES ('Liste des Usagers','3','98')  -- UserListLabel
INSERT INTO Translations VALUES ('Gestion des Usagers','3','99')  -- ManageUsersLabel
INSERT INTO Translations VALUES ('Nom Usager:','3','100')  -- UsernameLabel
INSERT INTO Translations VALUES ('Nom Docteur:','3','101')  -- DoctorsNameLabel
INSERT INTO Translations VALUES ('Mot de passe actuel:','3','102')  -- CurrentPasswordLabel
INSERT INTO Translations VALUES ('Mot de passe:','3','103')  -- PasswordLabel
INSERT INTO Translations VALUES ('Confirmation du mot de passe:','3','104')  -- ConfirmPasswordLabel
INSERT INTO Translations VALUES ('Les mots de passe ne corespondent pas!','3','105')  -- PasswordsDontMatchLabel
INSERT INTO Translations VALUES ('Le mot de passe doit contenir au moins 8 caractères et un chiffre.','3','106')  -- PasswordInvalidLabel
INSERT INTO Translations VALUES ('Admin','3','107')  -- AdminLabel
INSERT INTO Translations VALUES ('existe déjà!','3','108')  -- UsernameAlreadyExistsText
INSERT INTO Translations VALUES ('Nom d’usager existant','3','109')  -- UsernameAlreadyExistsTitle
INSERT INTO Translations VALUES ('existe déjà, mais est inactif.  Voulez-vous le réactiver ?','3','110')  -- UsernameAlreadyExistsInactive
INSERT INTO Translations VALUES ('L’usager :','3','111')  -- TheUsername
INSERT INTO Translations VALUES ('Réactiver l’usager ?','3','112')  -- ReactivateUserTitle
INSERT INTO Translations VALUES ('Le nom de docteur :','3','113')  -- ThePhysicianName
INSERT INTO Translations VALUES ('Voulez-vous vraiment supprimer l’usager :','3','114')  -- DeleteUserMessage
INSERT INTO Translations VALUES ('Supprimer Usager','3','115')  -- DeleteUserTitle
-- *****************************************************************************************
-- Home SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Fermer','3','116')  -- NewUserLabel
INSERT INTO Translations VALUES ('Changer de Réservoir','3','117')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Cryothérapie','3','118')  -- EditUserLabel
INSERT INTO Translations VALUES ('Enregistrements','3','119')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Parametres','3','120')  -- DeleteUserLabel

-- *****************************************************************************************
-- SETTINGS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('PRÉFÉRENCES MINUTEURS','3','121')  -- TimerSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Temps de refroidissement à :','3','122')  -- CoolingTimerToTextBlock
INSERT INTO Translations VALUES ('Temps de dégel :','3','123')  -- ThawTimerToTextBlock
INSERT INTO Translations VALUES ('Temps d’ablation :','3','124')  -- AblationTimerTextBlock
INSERT INTO Translations VALUES ('secondes','3','125')  -- SecondsTextBlock
INSERT INTO Translations VALUES ('Type de Graphique','3','126')  -- ChartTypeTextBlock
INSERT INTO Translations VALUES ('Couleur de la courbe','3','127')  -- CurveColorTextBlock
INSERT INTO Translations VALUES ('ALERT SETTINGS PREFERENCES','3','128')  -- AlertSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Low Ablation Temperature:','3','129')  -- LowAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('High Ablation Temperature:','3','130')  -- HighAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Température Oesophage:','3','131')  -- EsophagusTemperatureTextBlock
INSERT INTO Translations VALUES ('Diaphragm Sensor Limit:','3','132')  -- DiaphragmSensorLimitTextBlock
INSERT INTO Translations VALUES ('Diaphragm Sensor Gain:','3','133')  -- DiaphragmSensorGainTextBlock
INSERT INTO Translations VALUES ('PARAMÈTRES SYSTÈME','3','134')  -- SystemSettingsLabel
INSERT INTO Translations VALUES ('Vitesse Inflation','3','135')  -- InflateSpeedTextBlock
INSERT INTO Translations VALUES ('Rapide','3','136')  -- FastTextBlock
INSERT INTO Translations VALUES ('Lent','3','137')  -- SlowTextBox
INSERT INTO Translations VALUES ('Activé','3','138')  -- OnTextBlock
INSERT INTO Translations VALUES ('Désactivé','3','139')  -- OffTextBlock
INSERT INTO Translations VALUES ('DMS','3','140')  -- DMSTextBlock
INSERT INTO Translations VALUES ('Ligne','3','141')  -- LineTextBlock
INSERT INTO Translations VALUES ('Aire','3','142')  -- AreaTextBlock
INSERT INTO Translations VALUES ('Niveau Régrigérant','3','143')  -- RefrigerantLevelTextBlock
INSERT INTO Translations VALUES ('Weight','3','144')  -- LbsTextBlock
INSERT INTO Translations VALUES ('Min','3','145')  -- MinTextBlock

-- *****************************************************************************************
-- LOGIN SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Connexion','3','146')  -- LoginLabel
INSERT INTO Translations VALUES ('Usager ou mot de passe invalide, veuillez réessayer!','3','147')  -- WrongUsernameOrPasswordLabel
INSERT INTO Translations VALUES ('OK','3','148')  -- OkButton

-- *****************************************************************************************
-- PATIENT SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Information du patient','3','149')  -- PatientInformationLabel
INSERT INTO Translations VALUES ('Identifiant du Patient','3','150')  -- PatientIdLabel
INSERT INTO Translations VALUES ('Prénom','3','151')  -- FirstNameLabel
INSERT INTO Translations VALUES ('Nom','3','152')  -- LastNameLabel
INSERT INTO Translations VALUES ('Genre','3','153')  -- GenderLabel
INSERT INTO Translations VALUES ('Masculin','3','154')  -- MaleLabel
INSERT INTO Translations VALUES ('Féminin','3','155')  -- FemaleLabel
INSERT INTO Translations VALUES ('Date de naisance','3','156')  -- BirthDateLabel
INSERT INTO Translations VALUES ('JJ','3','157')  -- DayLabel
INSERT INTO Translations VALUES ('MM','3','158')  -- MonthLabel
INSERT INTO Translations VALUES ('AAAA','3','159')  -- YearLabel
INSERT INTO Translations VALUES ('Docteur','3','160')  -- PhysicianLabel

-- *****************************************************************************************
-- GENERIC SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('OUI','3','161')  -- YESButton
INSERT INTO Translations VALUES ('NON','3','162')  -- NOButton
INSERT INTO Translations VALUES ('Oui','3','163')  -- YesButton
INSERT INTO Translations VALUES ('Non','3','164')  -- NoButton
INSERT INTO Translations VALUES ('OUI','3','165')  -- YESLabel
INSERT INTO Translations VALUES ('NON','3','166')  -- NOLabel
INSERT INTO Translations VALUES ('Oui','3','167')  -- YesLabel
INSERT INTO Translations VALUES ('Non','3','168')  -- NoLabel

-- *****************************************************************************************
-- MESSAGE POPUP SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('MESSAGE SYSTÈME','3','169')  -- SystemMessageLabel
INSERT INTO Translations VALUES ('MESSAGE AVERTISSEMENT','3','170')  -- WarningMessageLabel
INSERT INTO Translations VALUES ('MESSAGE ERREUR','3','171')  -- ErrorMessageLabel

-- *****************************************************************************************
-- SETTINGS MAIN SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Date et Heure','3','172')  -- DateTimeLabel
INSERT INTO Translations VALUES ('Manuel Utilisateur','3','173')  -- UserManualLabel
INSERT INTO Translations VALUES ('Maintenance','3','174')  -- MaintenanceLabel
INSERT INTO Translations VALUES ('Registre Actions','3','175')  -- ActionLogLabel
INSERT INTO Translations VALUES ('JOURS','3','176')  -- DaysLabel
INSERT INTO Translations VALUES ('HEURES','3','177')  -- HoursLabel
INSERT INTO Translations VALUES ('MINUTES','3','178')  -- MinutesLabel

-- *****************************************************************************************
-- Add new Translation here
-- *****************************************************************************************
INSERT INTO Translations VALUES ('THE DMS IS OFF','3','179')  -- NoPacingDetectedLabelOff
INSERT INTO Translations VALUES ('Poid','3','180')  -- WeightLabel
INSERT INTO Translations VALUES ('Grandeur','3','181')  -- HeightLabel
INSERT INTO Translations VALUES ('Temps d’Ablation Actuel:','3','182')  -- ActualAblationTimerTextBlock
INSERT INTO Translations VALUES ('SU Temps Isolation Veine >','3','183')  -- ExpectedVeinIsolationTimeTextBlock
INSERT INTO Translations VALUES ('ALORS Fixer Temps d’Ablation =','3','184')  -- NewAblationTimerTextBlock
INSERT INTO Translations VALUES ('LOGIQUE ISOLATION VEINE','3','185')  -- VeinIsolationLogicLabel
INSERT INTO Translations VALUES ('Modifier Durée Isolation Veine ','3','186')  -- UpdateVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Nouvelle Durée','3','187')  -- NewVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Durée Invalide!','3','188')  -- InvalidDurationLabel
INSERT INTO Translations VALUES ('Temps d’Ablation','3','189')  -- AblationTimersLabel
INSERT INTO Translations VALUES ('Durée Fixe:','3','190')  -- FixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI Durée Fixe:','3','191')  -- TTIFixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI + Durée Fixe:','3','192')  -- TTIDurationTimerTextBlock
INSERT INTO Translations VALUES ('Temps Ablation:  TTI + ','3','193')  -- AblationTimerTTIPlusTextBlock

INSERT INTO Translations VALUES ('Température','3','194')  -- CryoBalloonTemperatureLabel
INSERT INTO Translations VALUES ('Sélectionnez un site d’ablation','3','195')  -- SelectAblationSiteLabel
INSERT INTO Translations VALUES ('Quitter le mode Lecture','3','196')  -- ExitPlaybackButton
INSERT INTO Translations VALUES ('Mode Lecture','3','197')  -- PlaybackModeLabel
INSERT INTO Translations VALUES ('Compléter la Procédure','3','198')  -- CompleteProcedureButton
INSERT INTO Translations VALUES ('Retour à la Procédure','3','199')  -- ReturnToProcedureButton
INSERT INTO Translations VALUES ('Fin de la Procédure','3','200')  -- EndProcedureButton
INSERT INTO Translations VALUES ('Lecture','3','201')  -- PlaybackButton
INSERT INTO Translations VALUES ('Date Opération','3','202')  -- CaseDateLabel
INSERT INTO Translations VALUES ('Sauvegarde sur USB','3','203')  -- SaveToUSBDriveLabel
INSERT INTO Translations VALUES ('Nom Volume :','3','204')  -- VolumeLabel
INSERT INTO Translations VALUES ('Nom :','3','205')  -- NameLabel
INSERT INTO Translations VALUES ('Format du Disque :','3','206')  -- DriveFormatLabel
INSERT INTO Translations VALUES ('Espace Libre (Octets) :','3','207')  -- FreeSpaceLabel
INSERT INTO Translations VALUES ('Espace Total (Octets) :','3','208')  -- TotalSizeLabel
INSERT INTO Translations VALUES ('Nom Fichier :','3','209')  -- FileNameLabel
INSERT INTO Translations VALUES ('Type Fichier :','3','210')  -- FileTypeLabel
INSERT INTO Translations VALUES ('Retour Au Traitement','3','211')  -- BackToTreatmentRecordButton
INSERT INTO Translations VALUES ('Utilisateur','3','212')  -- UserLabel
INSERT INTO Translations VALUES ('Accueil','3','213')  -- HomeLabel
INSERT INTO Translations VALUES ('Type Accès','3','214')  -- AccessTypeLabel
INSERT INTO Translations VALUES ('Alors','3','215')  -- ThenTextBlock
INSERT INTO Translations VALUES ('Autre','3','216')  -- ElseTextBlock
INSERT INTO Translations VALUES ('Choisir une Date…','3','217')  -- SelectADateLabel
INSERT INTO Translations VALUES ('Personnel Qualifié Seulement','3','218')  -- QualifiedPersonLabel
INSERT INTO Translations VALUES ('Message','3','219')  -- MessageLabel
INSERT INTO Translations VALUES ('Réinitialisation Système','3','220')  -- ResetSystemButton
INSERT INTO Translations VALUES ('Fermer','3','221')  -- CloseButton
INSERT INTO Translations VALUES ('Action Requise','3','222')  -- ActionRequiredLabel
INSERT INTO Translations VALUES ('Modifier Durée','3','223')  -- UpdateDurationLabel
INSERT INTO Translations VALUES ('Nouvelle Durée','3','224')  -- NewDurationLabel
INSERT INTO Translations VALUES ('Tout changement sera sauvegardé.','3','225')  -- UpdateAblationSiteWarningLabel
INSERT INTO Translations VALUES ('Plusieurs fichiers sélectionnés.  La sauvegarde sera dans :','3','226') -- MultipleFilesSelectedLabel
INSERT INTO Translations VALUES ('Erreur','3','227') -- ErrorLabel
INSERT INTO Translations VALUES ('Invalide! ','3','228') -- InvalidDMSThresholdLabel
INSERT INTO Translations VALUES ('Min: ','3','229') -- MinLabel
INSERT INTO Translations VALUES ('Max: ','3','230') -- MaxLabel
INSERT INTO Translations VALUES ('Alerte Audio','3','231') -- AudioAlertTextBlock
-- *****************************************************************************************
-- FRENCH TRANSLATION - END
-- *****************************************************************************************



-- *****************************************************************************************
-- SPANISH TRANSLATION - BEGIN
-- *****************************************************************************************
-- CRYOTHERAPY SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Encender ','6','1')  -- StartButton
INSERT INTO Translations VALUES ('Detener','6','2')  -- StopButton
INSERT INTO Translations VALUES ('VACÍO ACTIVADO','6','3')  -- ConnectButton
INSERT INTO Translations VALUES ('VACÍO DESACTIVADO','6','4')  -- DisconnectButton
INSERT INTO Translations VALUES ('Vena aislada','6','5')  -- VeinIsolationTextBlock
INSERT INTO Translations VALUES ('Configuración','6','6')  -- NotificationTextBlock
INSERT INTO Translations VALUES ('Notas ','6','7')  -- NotesTextBlock
INSERT INTO Translations VALUES ('Deshinchar al descongelar','6','8')  -- DeflateAtThawTextBlock
INSERT INTO Translations VALUES ('ESTADO:','6','9')  -- STATUSLabel
INSERT INTO Translations VALUES ('INACTIVO','6','10')  -- IDLELabel
INSERT INTO Translations VALUES ('LISTO','6','11') -- READYLabel
INSERT INTO Translations VALUES ('DISTENSIÓN','6','12')  -- INFLATIONLabel
INSERT INTO Translations VALUES ('ABLACIÓN','6','13')  -- ABLATIONLabel
INSERT INTO Translations VALUES ('DESCONGELACIÓN','6','14')  -- THAWINGLabel
INSERT INTO Translations VALUES ('RESUMEN DE LA ABLACIÓN','6','15')  -- ABLATIONSUMMARYLabel
INSERT INTO Translations VALUES ('Zona de la ablación','6','16')  -- AblationSiteLabel
INSERT INTO Translations VALUES ('Ablaciones','6','17')  -- AblationsLabel
INSERT INTO Translations VALUES ('Duración (s)','6','18')  -- DurationInSecLabel
INSERT INTO Translations VALUES ('RSPV','6','19')  -- RSPVLabel
INSERT INTO Translations VALUES ('RIPV','6','20')  -- RIPVLabel
INSERT INTO Translations VALUES ('LSPV','6','21')  -- LSPVLabel
INSERT INTO Translations VALUES ('LIPV','6','22')  -- LIPVLabel
INSERT INTO Translations VALUES ('OTROS','6','23')  -- OTHERLabel
INSERT INTO Translations VALUES ('Total:','6','24')  -- TotalLabel
INSERT INTO Translations VALUES ('Tiempo transcurrido:','6','25')  -- ElapsedTimeLabel
INSERT INTO Translations VALUES ('min','6','26')  -- minLabel
INSERT INTO Translations VALUES ('Hora actual:','6','27')  -- CurrentTimeLabel
INSERT INTO Translations VALUES ('TEMPERATURA','6','28')  -- TEMPERATURELabel
INSERT INTO Translations VALUES ('TIEMPO DE ABLACIÓN','6','29')  -- AblationTimeLabel
INSERT INTO Translations VALUES ('TEMPORIZADORES Y FRECUENCIAS','6','30')  -- TIMERSAndRATESLabel
INSERT INTO Translations VALUES ('ALERTAS E INDICADORES','6','31')  -- ALERTSAndINDICATORSLabel
INSERT INTO Translations VALUES ('INFORMACIÓN DEL SISTEMA','6','32')  -- SystemINFOLabel
INSERT INTO Translations VALUES ('VARIACIÓN DE TEMPERATURA','6','33')  -- TEMPERATUREAndRATELabel
INSERT INTO Translations VALUES ('TEMPERATURA MÍNIMA','6','34')  -- MinimumTEMPERATURELabel
INSERT INTO Translations VALUES ('TIEMPO DE ENFRIAMIENTO HASTA','6','35')  -- CoolingTimeToLabel
INSERT INTO Translations VALUES ('TIEMPO HASTA HACER EFECTO','6','36')  -- TimeToEffectLabel
INSERT INTO Translations VALUES ('TIEMPO DE DESCONGELACIÓN HASTA','6','37')  -- ThawTimeToLabel
INSERT INTO Translations VALUES ('DIAFRAGMA','6','38')  -- DiaphragmLabel
INSERT INTO Translations VALUES ('MOVIMIENTO','6','39')  -- MovementLabel
INSERT INTO Translations VALUES ('Zoom:','6','40')  -- ZoomLabel
INSERT INTO Translations VALUES ('NO SE HA DETECTADO ELECTROESTIMULACIÓN CARDÍACA','6','41')  -- NoPacingDetectedLabel
INSERT INTO Translations VALUES ('Solo como referencia. Nunca confíe únicamente en estos indicadores.','6','42')  -- ForReferenceOnlyNeverRelySolelyOnTheseIndicatorsLabel
INSERT INTO Translations VALUES ('ESÓFAGO','6','43')  -- ESOPHAGUSLabel
INSERT INTO Translations VALUES ('TEMPERATURA','6','44')  -- TEMPERATURELabel1
INSERT INTO Translations VALUES ('De 10 °C a 40 °C','6','45')  -- TemperatureRangeLabel
INSERT INTO Translations VALUES ('FLUJO:','6','46')  -- FlowLabel
INSERT INTO Translations VALUES ('PRESIÓN:','6','47')  -- PRESSURELabel
INSERT INTO Translations VALUES ('BALÓN:','6','48')  -- BalloonLabel
INSERT INTO Translations VALUES ('Duración de la ablación','6','49')  -- TimerLabel
INSERT INTO Translations VALUES ('seg','6','50')  -- SecLabel
INSERT INTO Translations VALUES ('Tratamiento:','6','51')  -- TreatmentLabel
INSERT INTO Translations VALUES ('de','6','52')  -- TreatmentNumberOf
INSERT INTO Translations VALUES ('El sistema se está iniciando...','6','53')  -- WaitSystemIsInitializing

-- *****************************************************************************************
-- TREATMENT RECORDS SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('REGISTROS DE LOS TRATAMIENTOS','6','54')  -- TreatmentRecordTitleLabel
INSERT INTO Translations VALUES ('INFORMACIÓN DEL PACIENTE','6','55')  -- PatientInfoLabel
INSERT INTO Translations VALUES ('Paciente','6','56')  -- PatientNameLabel
INSERT INTO Translations VALUES ('Sexo','6','57')  -- PatientGenderLabel
INSERT INTO Translations VALUES ('Fecha de nacimiento','6','58')  -- PatientBirthDateLabel
INSERT INTO Translations VALUES ('Número de identificación','6','59')  -- PatientIdNumberLabel
INSERT INTO Translations VALUES ('INFORMACIÓN DE PROCEDIMIENTO','6','60')  -- ProcedureInfoLabel
INSERT INTO Translations VALUES ('MÉDICO','6','61')  -- PhysicianNameLabel
INSERT INTO Translations VALUES ('Catéter utilizado','6','62')  -- CatheterUsedLabel
INSERT INTO Translations VALUES ('Fecha del procedimiento','6','63')  -- ProcedureDateLabel
INSERT INTO Translations VALUES ('TEMPERATURA DEL ESÓFAGO','6','64')  -- EsophagusTemperatureLabel
INSERT INTO Translations VALUES ('MOVIMIENTO DEL DIAFRAGMA','6','65')  -- DiaphragmMovementLabel
INSERT INTO Translations VALUES ('PRESIÓN DEL BALÓN','6','66')  -- BalloonPressureLabel
INSERT INTO Translations VALUES ('REGISTROS DEL PROCEDIMIENTO','6','67')  -- ProcedureRecordsLabel
INSERT INTO Translations VALUES ('Guardar en USB','6','68')  -- ExportProcedureButton
INSERT INTO Translations VALUES ('Guardado de datos de ingeniería en curso...','6','69')  -- SaveEngineeringDataInProgressLabel

-- *****************************************************************************************
-- SUMMARY REPORT SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('INFORME DE RESUMEN','6','70')  -- SummaryReportLabel
INSERT INTO Translations VALUES ('INFORMACIÓN DEL TRATAMIENTO','6','71')  -- TreatmentInfoLabel
INSERT INTO Translations VALUES ('Diagnóstico','6','72')  -- DiagnosisLabel
INSERT INTO Translations VALUES ('Resultado','6','73')  -- OutcomeLabel

-- *****************************************************************************************
-- CHANGE TANK SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('REEMPLAZO DEL TANQUE','6','74')  -- TankReplacementLabel
INSERT INTO Translations VALUES ('Cierre el tanque y, a continuación, presione Siguiente','6','75')  -- CloseTankLabel
INSERT INTO Translations VALUES ('Para su seguridad, espere hasta que la consola haya purgado la vía','6','76')  -- WaitLabel
INSERT INTO Translations VALUES ('Reemplace el tanque','6','77')  -- ReplaceTankLabel
INSERT INTO Translations VALUES ('Abra el tanque','6','78')  -- OpenTankLabel
INSERT INTO Translations VALUES ('Siga las instrucciones para un reemplazo seguro del tanque','6','79')  -- FollowInstructionsLabel
INSERT INTO Translations VALUES ('Se ha cambiado correctamente el tanque!','6','80')  -- ChangeTankSuccessLabel
INSERT INTO Translations VALUES ('Siguiente','6','81')  -- NextButton
INSERT INTO Translations VALUES ('Finalizar','6','82')  -- FinishButton
INSERT INTO Translations VALUES ('Cancelar','6','83')  -- CancelButton
INSERT INTO Translations VALUES ('Fecha del cambio :','6','84')  -- ChangeDateLabel
INSERT INTO Translations VALUES ('Peso en el momento del cambio :','6','85')  -- WeightAtChangeLabel
INSERT INTO Translations VALUES ('Peso actual :','6','86')  -- CurrentWeightLabel
INSERT INTO Translations VALUES ('Seleccione un tipo de tanque de reemplazo :','6','87')  -- SelectTankReplacementTypeLabel
INSERT INTO Translations VALUES ('10 libras','6','88')  -- TenPoundsLabel
INSERT INTO Translations VALUES ('15 libras','6','89')  -- FifteenPoundsLabel

-- *****************************************************************************************
-- MANAGE USERS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Nuevo usuario','6','90')  -- NewUserLabel
INSERT INTO Translations VALUES ('Nuevo médico','6','91')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Editar usuario','6','92')  -- EditUserLabel
INSERT INTO Translations VALUES ('Editar médico','6','93')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Eliminar usuario','6','94')  -- DeleteUserLabel
INSERT INTO Translations VALUES ('Eliminar médico','6','95')  -- DeleteDoctorLabel
INSERT INTO Translations VALUES ('Restablecer contraseña','6','96')  -- ResetPasswordLabel
INSERT INTO Translations VALUES ('Volver a Configuración','6','97')  -- ReturnToSettingsButton
INSERT INTO Translations VALUES ('Lista de usuarios','6','98')  -- UserListLabel
INSERT INTO Translations VALUES ('Administrar usuarios','6','99')  -- ManageUsersLabel
INSERT INTO Translations VALUES ('Nombre de usuario:','6','100')  -- UsernameLabel
INSERT INTO Translations VALUES ('Nombre del médico:','6','101')  -- DoctorsNameLabel
INSERT INTO Translations VALUES ('Contraseña actual:','6','102')  -- CurrentPasswordLabel
INSERT INTO Translations VALUES ('Contraseña:','6','103')  -- PasswordLabel
INSERT INTO Translations VALUES ('Confirmar contraseña:','6','104')  -- ConfirmPasswordLabel
INSERT INTO Translations VALUES ('Las contraseñas no coinciden!','6','105')  -- PasswordsDontMatchLabel
INSERT INTO Translations VALUES ('Las contraseñas deben contener, por lo menos, ocho caracteres y un dígito.','6','106')  -- PasswordInvalidLabel
INSERT INTO Translations VALUES ('Administrador','6','107')  -- AdminLabel
INSERT INTO Translations VALUES ('ya existe!','6','108')  -- UsernameAlreadyExistsText
INSERT INTO Translations VALUES ('Este nombre de usuario ya existe','6','109')  -- UsernameAlreadyExistsTitle
INSERT INTO Translations VALUES ('ya existe, pero el usuario está inactivo.  ¿Desea reactivarlo?','6','110')  -- UsernameAlreadyExistsInactive
INSERT INTO Translations VALUES ('Nombre de usuario:','6','111')  -- TheUsername
INSERT INTO Translations VALUES ('¿Desea reactivar el usuario?','6','112')  -- ReactivateUserTitle
INSERT INTO Translations VALUES ('Nombre del médico :','6','113')  -- ThePhysicianName
INSERT INTO Translations VALUES ('¿Está seguro de que desea eliminar el usuario? :','6','114')  -- DeleteUserMessage
INSERT INTO Translations VALUES ('Eliminar usuario','6','115')  -- DeleteUserTitle

-- *****************************************************************************************
-- Home SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Apagar','6','116')  -- NewUserLabel
INSERT INTO Translations VALUES ('Cambiar tanque','6','117')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Crioterapia','6','118')  -- EditUserLabel
INSERT INTO Translations VALUES ('Registros','6','119')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Configuración','6','120')  -- DeleteUserLabel

-- *****************************************************************************************
-- SETTINGS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('PREFERENCIAS DE CONFIGURACIÓN DEL TEMPORIZADOR','6','121')  -- TimerSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Temporizador de enfriamiento a:','6','122')  -- CoolingTimerToTextBlock
INSERT INTO Translations VALUES ('Descongelar el temporizador para:','6','123')  -- ThawTimerToTextBlock
INSERT INTO Translations VALUES ('Temporizador de ablación:','6','124')  -- AblationTimerTextBlock
INSERT INTO Translations VALUES ('segundos','6','125')  -- SecondsTextBlock
INSERT INTO Translations VALUES ('Tipo de gráfico','6','126')  -- ChartTypeTextBlock
INSERT INTO Translations VALUES ('Color de Curva','6','127')  -- CurveColorTextBlock

INSERT INTO Translations VALUES ('PREFERENCIAS DE CONFIGURACIÓN DE ALERTA','6','128')  -- AlertSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Baja temperatura de ablación:','6','129')  -- LowAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Alta temperatura de ablación:','6','130')  -- HighAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Temperatura del esófago:','6','131')  -- EsophagusTemperatureTextBlock
INSERT INTO Translations VALUES ('Límite del sensor de diafragma:','6','132')  -- DiaphragmSensorLimitTextBlock
INSERT INTO Translations VALUES ('Ganancia del sensor de diafragma:','6','133')  -- DiaphragmSensorGainTextBlock

INSERT INTO Translations VALUES ('AJUSTES DEL SISTEMA','6','134')  -- SystemSettingsLabel
INSERT INTO Translations VALUES ('Velocidad de inflado','6','135')  -- InflateSpeedTextBlock
INSERT INTO Translations VALUES ('Rápido','6','136')  -- FastTextBlock
INSERT INTO Translations VALUES ('Lento','6','137')  -- SlowTextBox
INSERT INTO Translations VALUES ('En','6','138')  -- OnTextBlock
INSERT INTO Translations VALUES ('Apagado','6','139')  -- OffTextBlock
INSERT INTO Translations VALUES ('DMS','6','140')  -- DMSTextBlock
INSERT INTO Translations VALUES ('Línea','6','141')  -- LineTextBlock
INSERT INTO Translations VALUES ('Zona','6','142')  -- AreaTextBlock
INSERT INTO Translations VALUES ('Nivel de refrigerante','6','143')  -- RefrigerantLevelTextBlock
INSERT INTO Translations VALUES ('Lbs','6','144')  -- LbsTextBlock
INSERT INTO Translations VALUES ('Min','6','145')  -- MinTextBlock

-- *****************************************************************************************
-- LOGIN SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Iniciar sesión','6','146')  -- LoginLabel
INSERT INTO Translations VALUES ('¡Nombre de usuario o contraseña equivocada, por favor intente otra vez!','6','147')  -- WrondUsernameOrPasswordLabel
INSERT INTO Translations VALUES ('DE ACUERDO','6','148')  -- OkButton

-- *****************************************************************************************
-- PATIENT SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Información del paciente','6','149')  -- PatientInformationLabel
INSERT INTO Translations VALUES ('ID del paciente','6','150')  -- PatientIdLabel
INSERT INTO Translations VALUES ('Nombre de pila','6','151')  -- FirstNameLabel
INSERT INTO Translations VALUES ('Apellido','6','152')  -- LastNameLabel
INSERT INTO Translations VALUES ('Sexo','6','153')  -- GenderLabel
INSERT INTO Translations VALUES ('Masculino','6','154')  -- MaleLabel
INSERT INTO Translations VALUES ('Hembra','6','155')  -- FemaleLabel
INSERT INTO Translations VALUES ('Fecha de nacimiento','6','156')  -- BirthDateLabel
INSERT INTO Translations VALUES ('DD','6','157')  -- DayLabel
INSERT INTO Translations VALUES ('MM','6','158')  -- MonthLabel
INSERT INTO Translations VALUES ('YYYY','6','159')  -- YearLabel
INSERT INTO Translations VALUES ('Médico','6','160')  -- PhysicianLabel

-- *****************************************************************************************
-- GENERIC SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Sí','6','161')  -- YESButton
INSERT INTO Translations VALUES ('NO','6','162')  -- NOButton
INSERT INTO Translations VALUES ('Sí','6','163')  -- YesButton
INSERT INTO Translations VALUES ('No','6','164')  -- NoButton
INSERT INTO Translations VALUES ('Sí','6','165')  -- YESLabel
INSERT INTO Translations VALUES ('NO','6','166')  -- NOLabel
INSERT INTO Translations VALUES ('Sí','6','167')  -- YesLabel
INSERT INTO Translations VALUES ('No','6','168')  -- NoLabel

-- *****************************************************************************************
-- MESSAGE POPUP SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('MENSAJE DEL SISTEMA','6','169')  -- SystemMessageLabel
INSERT INTO Translations VALUES ('MENSAJE DE ADVERTENCIA','6','170')  -- WarningMessageLabel
INSERT INTO Translations VALUES ('MENSAJE DE ERROR','6','171')  -- ErrorMessageLabel

-- *****************************************************************************************
-- SETTINGS MAIN SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Hora y fecha','6','172')  -- DateTimeLabel
INSERT INTO Translations VALUES ('Manual de usuario','6','173')  -- UserManualLabel
INSERT INTO Translations VALUES ('Mantenimiento','6','174')  -- MaintenanceLabel
INSERT INTO Translations VALUES ('Registro de Acción','6','175')  -- ActionLogLabel
INSERT INTO Translations VALUES ('DIAS','6','176')  -- DaysLabel
INSERT INTO Translations VALUES ('HORAS','6','177')  -- HoursLabel
INSERT INTO Translations VALUES ('MONUTOS','6','178')  -- MinutesLabel

-- *****************************************************************************************
-- Add new Translation here
-- *****************************************************************************************
INSERT INTO Translations VALUES ('THE DMS IS OFF','6','179')  -- NoPacingDetectedLabelOff
INSERT INTO Translations VALUES ('Peso','6','180')  -- WeightLabel
INSERT INTO Translations VALUES ('Altura','6','181')  -- HeightLabel
INSERT INTO Translations VALUES ('Current Ablation Timer:','6','182')  -- ActualAblationTimerTextBlock
INSERT INTO Translations VALUES ('IF Vein Isolation Time >','6','183')  -- ExpectedVeinIsolationTimeTextBlock
INSERT INTO Translations VALUES ('THEN Set Ablation Timer =','6','184')  -- NewAblationTimerTextBlock
INSERT INTO Translations VALUES ('VEIN ISOLATION LOGIC','6','185')  -- VeinIsolationLogicLabel
INSERT INTO Translations VALUES ('Actualizar la duración del aislamiento de venas','6','186')  -- UpdateVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Nueva duración','6','187')  -- NewVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Duración no válida!','6','188')  -- InvalidDurationLabel
INSERT INTO Translations VALUES ('Temporizador de Ablación','6','189')  -- AblationTimersLabel
INSERT INTO Translations VALUES ('Temporizador Fijo:','6','190')  -- FixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI Temporizador Fijo:','6','191')  -- TTIFixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI + temporizador de duración:','6','192')  -- TTIDurationTimerTextBlock
INSERT INTO Translations VALUES ('Temporizador de Ablación:  TTI + ','6','193')  -- AblationTimerTTIPlusTextBlock

INSERT INTO Translations VALUES ('Temperatura','6','194')  -- CryoBalloonTemperatureLabel
INSERT INTO Translations VALUES ('Seleccione un sitio de ablación','6','195')  -- SelectAblationSiteLabel
INSERT INTO Translations VALUES ('Salir de la reproducción','6','196')  -- ExitPlaybackButton
INSERT INTO Translations VALUES ('Modo de reproducción','6','197')  -- PlaybackModeLabel
INSERT INTO Translations VALUES ('Procedimiento Completo','6','198')  -- CompleteProcedureButton
INSERT INTO Translations VALUES ('Volver al Procedimiento','6','199')  -- ReturnToProcedureButton
INSERT INTO Translations VALUES ('Procedimiento final','6','200')  -- EndProcedureButton
INSERT INTO Translations VALUES ('Reproducción','6','201')  -- PlaybackButton
INSERT INTO Translations VALUES ('Fecha del caso','6','202')  -- CaseDateLabel
INSERT INTO Translations VALUES ('GUARDAR EN UNIDAD USB','6','203')  -- SaveToUSBDriveLabel
INSERT INTO Translations VALUES ('Etiqueta de volumen:','6','204')  -- VolumeLabel
INSERT INTO Translations VALUES ('Nombre:','6','205')  -- NameLabel
INSERT INTO Translations VALUES ('Formato de unidad:','6','206')  -- DriveFormatLabel
INSERT INTO Translations VALUES ('Espacio libre (bytes):','6','207')  -- FreeSpaceLabel
INSERT INTO Translations VALUES ('Tamaño total (bytes):','6','208')  -- TotalSizeLabel
INSERT INTO Translations VALUES ('Nombre del archivo:','6','209')  -- FileNameLabel
INSERT INTO Translations VALUES ('Tipo de archivo:','6','210')  -- FileTypeLabel
INSERT INTO Translations VALUES ('Volver al registro de tratamiento','6','211')  -- BackToTreatmentRecordButton
INSERT INTO Translations VALUES ('Usuario','6','212')  -- UserLabel
INSERT INTO Translations VALUES ('Casa','6','213')  -- HomeLabel
INSERT INTO Translations VALUES ('Tipo de acceso','6','214')  -- AccessTypeLabel
INSERT INTO Translations VALUES ('Entonces','6','215')  -- ThenTextBlock
INSERT INTO Translations VALUES ('Más','6','216')  -- ElseTextBlock
INSERT INTO Translations VALUES ('Seleccione una fecha ...','6','217')  -- SelectADateLabel
INSERT INTO Translations VALUES ('Persona calificada solamente','6','218')  -- QualifiedPersonLabel
INSERT INTO Translations VALUES ('Mensaje','6','219')  -- MessageLabel
INSERT INTO Translations VALUES ('Restablecer sistema','6','220')  -- ResetSystemButton
INSERT INTO Translations VALUES ('Cerca','6','221')  -- CloseButton
INSERT INTO Translations VALUES ('Acción requerida','6','222')  -- ActionRequiredLabel
INSERT INTO Translations VALUES ('Duración de actualización','6','223')  -- UpdateDurationLabel
INSERT INTO Translations VALUES ('Nueva duración','6','224')  -- NewDurationLabel
INSERT INTO Translations VALUES ('Cualquier cambio en el sitio de ablación se guardará.','6','225')  -- UpdateAblationSiteWarningLabel
INSERT INTO Translations VALUES ('Múltiples archivos seleccionados Se guardará en:','6','226') -- MultipleFilesSelectedLabel
INSERT INTO Translations VALUES ('Error','6','227') -- MultipleFilesSelectedLabel
INSERT INTO Translations VALUES ('Inválido! ','6','228') -- InvalidDMSThresholdLabel
INSERT INTO Translations VALUES ('Min: ','6','229') -- MinLabel
INSERT INTO Translations VALUES ('Max: ','6','230') -- MaxLabel
INSERT INTO Translations VALUES ('Alerta de audio','6','231') -- AudioAlertTextBlock
-- *****************************************************************************************
-- SPANISH TRANSLATION - END
-- *****************************************************************************************


-- *****************************************************************************************
-- ITALIAN TRANSLATION - BEGIN
-- *****************************************************************************************
-- CRYOTHERAPY SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Avvio ','7','1')  -- StartButton
INSERT INTO Translations VALUES ('Arresta','7','2')  -- StopButton
INSERT INTO Translations VALUES ('VUOTO ABILITATO','7','3')  -- ConnectButton
INSERT INTO Translations VALUES ('VUOTO DISABILITATO','7','4')  -- DisconnectButton
INSERT INTO Translations VALUES ('Vena isolata','7','5')  -- VeinIsolationTextBlock
INSERT INTO Translations VALUES ('Impostazioni','7','6')  -- NotificationTextBlock
INSERT INTO Translations VALUES ('Note ','7','7')  -- NotesTextBlock
INSERT INTO Translations VALUES ('Sgonfiare durante lo scongelamento','7','8')  -- DeflateAtThawTextBlock
INSERT INTO Translations VALUES ('STATO:','7','9')  -- STATUSLabel
INSERT INTO Translations VALUES ('INATTIVO','7','10')  -- IDLELabel
INSERT INTO Translations VALUES ('PRONTO','7','11') -- READYLabel
INSERT INTO Translations VALUES ('GONFIAGGIO','7','12')  -- INFLATIONLabel
INSERT INTO Translations VALUES ('ABLAZIONE','7','13')  -- ABLATIONLabel
INSERT INTO Translations VALUES ('SCONGELAMENTO','7','14')  -- THAWINGLabel
INSERT INTO Translations VALUES ('RIEPILOGO ABLAZIONE','7','15')  -- ABLATIONSUMMARYLabel
INSERT INTO Translations VALUES ('Sito di ablazione','7','16')  -- AblationSiteLabel
INSERT INTO Translations VALUES ('Ablazioni','7','17')  -- AblationsLabel
INSERT INTO Translations VALUES ('Durata (s)','7','18')  -- DurationInSecLabel
INSERT INTO Translations VALUES ('RSPV','7','19')  -- RSPVLabel
INSERT INTO Translations VALUES ('RIPV','7','20')  -- RIPVLabel
INSERT INTO Translations VALUES ('LSPV','7','21')  -- LSPVLabel
INSERT INTO Translations VALUES ('LIPV','7','22')  -- LIPVLabel
INSERT INTO Translations VALUES ('ALTRO','7','23')  -- OTHERLabel
INSERT INTO Translations VALUES ('Totale:','7','24')  -- TotalLabel
INSERT INTO Translations VALUES ('Tempo trascorso:','7','25')  -- ElapsedTimeLabel
INSERT INTO Translations VALUES ('min','7','26')  -- minLabel
INSERT INTO Translations VALUES ('Ora attuale:','7','27')  -- CurrentTimeLabel
INSERT INTO Translations VALUES ('TEMPERATURA','7','28')  -- TEMPERATURELabel
INSERT INTO Translations VALUES ('TEMPO DI ABLAZIONE','7','29')  -- AblationTimeLabel
INSERT INTO Translations VALUES ('TIMER E TASSI','7','30')  -- TIMERSAndRATESLabel
INSERT INTO Translations VALUES ('AVVISI E INDICATORI','7','31')  -- ALERTSAndINDICATORSLabel
INSERT INTO Translations VALUES ('INFORMAZIONI SISTEMA','7','32')  -- SystemINFOLabel
INSERT INTO Translations VALUES ('TASSO DI TEMPERATURA','7','33')  -- TEMPERATUREAndRATELabel
INSERT INTO Translations VALUES ('TEMPERATURA MINIMA','7','34')  -- MinimumTEMPERATURELabel
INSERT INTO Translations VALUES ('TEMPO DI RAFFREDDAMENTO PER','7','35')  -- CoolingTimeToLabel
INSERT INTO Translations VALUES ('TEMPO PER AVERE EFFETTO','7','36')  -- TimeToEffectLabel
INSERT INTO Translations VALUES ('PERIODO DI SCONGELAMENTO PER','7','37')  -- ThawTimeToLabel
INSERT INTO Translations VALUES ('DIAFRAMMA','7','38')  -- DiaphragmLabel
INSERT INTO Translations VALUES ('SPOSTAMENTO','7','39')  -- MovementLabel
INSERT INTO Translations VALUES ('Zoom:','7','40')  -- ZoomLabel
INSERT INTO Translations VALUES ('NESSUNA VELOCITÀ RILEVATA','7','41')  -- NoPacingDetectedLabel
INSERT INTO Translations VALUES ('Solo per riferimento. Non affidarsi mai esclusivamente a questi indicatori','7','42')  -- ForReferenceOnlyNeverRelySolelyOnTheseIndicatorsLabel
INSERT INTO Translations VALUES ('ESOFAGO','7','43')  -- ESOPHAGUSLabel
INSERT INTO Translations VALUES ('TEMPERATURA','7','44')  -- TEMPERATURELabel1
INSERT INTO Translations VALUES ('Da 10 °C a 40 °C','7','45')  -- TemperatureRangeLabel
INSERT INTO Translations VALUES ('FLUSSO:','7','46')  -- FlowLabel
INSERT INTO Translations VALUES ('PRESSIONE:','7','47')  -- PRESSURELabel
INSERT INTO Translations VALUES ('PALLONCINO:','7','48')  -- BalloonLabel
INSERT INTO Translations VALUES ('Durata di ablazione','7','49')  -- TimerLabel
INSERT INTO Translations VALUES ('s','7','50')  -- SecLabel
INSERT INTO Translations VALUES ('Trattamento:','7','51')  -- TreatmentLabel
INSERT INTO Translations VALUES ('di','7','52')  -- TreatmentNumberOf
INSERT INTO Translations VALUES ('Si prega di attendere, inizializzazione del sistema in corso...','7','53')  -- WaitSystemIsInitializing

-- *****************************************************************************************
-- TREATMENT RECORDS SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('RECORD TRATTAMENTO','7','54')  -- TreatmentRecordTitleLabel
INSERT INTO Translations VALUES ('INFORMAZIONI PAZIENTE','7','55')  -- PatientInfoLabel
INSERT INTO Translations VALUES ('Paziente','7','56')  -- PatientNameLabel
INSERT INTO Translations VALUES ('Sesso','7','57')  -- PatientGenderLabel
INSERT INTO Translations VALUES ('Data di nascita','7','58')  -- PatientBirthDateLabel
INSERT INTO Translations VALUES ('Numero ID','7','59')  -- PatientIdNumberLabel
INSERT INTO Translations VALUES ('INFORMAZIONI PROCEDURA','7','60')  -- ProcedureInfoLabel
INSERT INTO Translations VALUES ('MEDICO','7','61')  -- PhysicianNameLabel
INSERT INTO Translations VALUES ('Catetere utilizzato','7','62')  -- CatheterUsedLabel
INSERT INTO Translations VALUES ('Data della procedura','7','63')  -- ProcedureDateLabel
INSERT INTO Translations VALUES ('TEMPERATURA ESOFAGO','7','64')  -- EsophagusTemperatureLabel
INSERT INTO Translations VALUES ('SPOSTAMENTO DIAFRAMMA','7','65')  -- DiaphragmMovementLabel
INSERT INTO Translations VALUES ('PRESSIONE PALLONCINO','7','66')  -- BalloonPressureLabel
INSERT INTO Translations VALUES ('RECORD PROCEDURA','7','67')  -- ProcedureRecordsLabel
INSERT INTO Translations VALUES ('Salva su USB','7','68')  -- ExportProcedureButton
INSERT INTO Translations VALUES ('Salvataggio dati ingegneria in corso...','7','69')  -- SaveEngineeringDataInProgressLabel

-- *****************************************************************************************
-- SUMMARY REPORT SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('RELAZIONE DI SINTESI','7','70')  -- SummaryReportLabel
INSERT INTO Translations VALUES ('INFORMAZIONI TRATTAMENTO','7','71')  -- TreatmentInfoLabel
INSERT INTO Translations VALUES ('Diagnosi','7','72')  -- DiagnosisLabel
INSERT INTO Translations VALUES ('Risultati','7','73')  -- OutcomeLabel

-- *****************************************************************************************
-- CHANGE TANK SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('SOSTITUZIONE SERBATOIO','7','74')  -- TankReplacementLabel
INSERT INTO Translations VALUES ('Chiudere il serbatoio e poi premere Avanti','7','75')  -- CloseTankLabel
INSERT INTO Translations VALUES ('Attendere mentre la console spurga il tubo per sicurezza','7','76')  -- WaitLabel
INSERT INTO Translations VALUES ('Sostituire il serbatoio','7','77')  -- ReplaceTankLabel
INSERT INTO Translations VALUES ('Aprire il serbatoio','7','78')  -- OpenTankLabel
INSERT INTO Translations VALUES ('Seguire le Istruzioni per la sostituzione sicura del serbatoio','7','79')  -- FollowInstructionsLabel
INSERT INTO Translations VALUES ('Cambio serbatoio riuscito !','7','80')  -- ChangeTankSuccessLabel
INSERT INTO Translations VALUES ('Avanti','7','81')  -- NextButton
INSERT INTO Translations VALUES ('Fine','7','82')  -- FinishButton
INSERT INTO Translations VALUES ('Annulla','7','83')  -- CancelButton
INSERT INTO Translations VALUES ('Modifica data :','7','84')  -- ChangeDateLabel
INSERT INTO Translations VALUES ('Peso alla modifica :','7','85')  -- WeightAtChangeLabel
INSERT INTO Translations VALUES ('Peso attuale :','7','86')  -- CurrentWeightLabel
INSERT INTO Translations VALUES ('Selezionare un tipo di serbatoio sostitutivo :','7','87')  -- SelectTankReplacementTypeLabel
INSERT INTO Translations VALUES ('4,5 chilogrammi','7','88')  -- TenPoundsLabel
INSERT INTO Translations VALUES ('6,75 chilogrammi','7','89')  -- FifteenPoundsLabel

-- *****************************************************************************************
-- MANAGE USERS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Nuovo utente','7','90')  -- NewUserLabel
INSERT INTO Translations VALUES ('Nuovo medico','7','91')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Modifica utente','7','92')  -- EditUserLabel
INSERT INTO Translations VALUES ('Modifica medico','7','93')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Elimina utente','7','94')  -- DeleteUserLabel
INSERT INTO Translations VALUES ('Elimina medico','7','95')  -- DeleteDoctorLabel
INSERT INTO Translations VALUES ('Reimposta password','7','96')  -- ResetPasswordLabel
INSERT INTO Translations VALUES ('Ritorna a Impostazioni','7','97')  -- ReturnToSettingsButton
INSERT INTO Translations VALUES ('Elenco utenti','7','98')  -- UserListLabel
INSERT INTO Translations VALUES ('Gestisci utenti','7','99')  -- ManageUsersLabel
INSERT INTO Translations VALUES ('Nome utente:','7','100')  -- UsernameLabel
INSERT INTO Translations VALUES ('Nome del medico:','7','101')  -- DoctorsNameLabel
INSERT INTO Translations VALUES ('Password attuale:','7','102')  -- CurrentPasswordLabel
INSERT INTO Translations VALUES ('Password:','7','103')  -- PasswordLabel
INSERT INTO Translations VALUES ('Conferma password:','7','104')  -- ConfirmPasswordLabel
INSERT INTO Translations VALUES ('Le password non corrispondono!','7','105')  -- PasswordsDontMatchLabel
INSERT INTO Translations VALUES ('Le password devono contenere almeno otto caratteri e una cifra.','7','106')  -- PasswordInvalidLabel
INSERT INTO Translations VALUES ('Amministratore','7','107')  -- AdminLabel
INSERT INTO Translations VALUES ('esiste già!','7','108')  -- UsernameAlreadyExistsText
INSERT INTO Translations VALUES ('Il nome utente esiste','7','109')  -- UsernameAlreadyExistsTitle
INSERT INTO Translations VALUES ('esiste già ma l’utente è inattivo. Desidera riattivarlo?','7','110')  -- UsernameAlreadyExistsInactive
INSERT INTO Translations VALUES ('Il nome utente:','7','111')  -- TheUsername
INSERT INTO Translations VALUES ('Riattivare utente?','7','112')  -- ReactivateUserTitle
INSERT INTO Translations VALUES ('Il nome del medico :','7','113')  -- ThePhysicianName
INSERT INTO Translations VALUES ('Desidera eliminare l’utente :','7','114')  -- DeleteUserMessage
INSERT INTO Translations VALUES ('Elimina utente','7','115')  -- DeleteUserTitle

-- *****************************************************************************************
-- Home SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Arresta','7','116')  -- NewUserLabel
INSERT INTO Translations VALUES ('Cambia serbatoio','7','117')  -- NewDoctorLabel
INSERT INTO Translations VALUES ('Crioterapia','7','118')  -- EditUserLabel
INSERT INTO Translations VALUES ('Record','7','119')  -- EditDoctorLabel
INSERT INTO Translations VALUES ('Impostazioni','7','120')  -- DeleteUserLabel

-- *****************************************************************************************
-- SETTINGS SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('PREFERENZE DELLE IMPOSTAZIONI DEL TIMER','7','121')  -- TimerSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Timer di raffreddamento A:','7','122')  -- CoolingTimerToTextBlock
INSERT INTO Translations VALUES ('Scongelare il timer a:','7','123')  -- ThawTimerToTextBlock
INSERT INTO Translations VALUES ('Timer di ablazione:','7','124')  -- AblationTimerTextBlock
INSERT INTO Translations VALUES ('secondi','7','125')  -- SecondsTextBlock
INSERT INTO Translations VALUES ('Tipo di grafico','7','126')  -- ChartTypeTextBlock
INSERT INTO Translations VALUES ('Colore curva','7','127')  -- CurveColorTextBlock

INSERT INTO Translations VALUES ('ALTRE IMPOSTAZIONI PREFERENZE','7','128')  -- AlertSettingsPreferencesLabel
INSERT INTO Translations VALUES ('Bassa temperatura di ablazione:','7','129')  -- LowAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Alta temperatura di ablazione:','7','130')  -- HighAblationTemperatureTextBlock
INSERT INTO Translations VALUES ('Temperatura dell esofago:','7','131')  -- EsophagusTemperatureTextBlock
INSERT INTO Translations VALUES ('Limite del sensore del diaframma:','7','132')  -- DiaphragmSensorLimitTextBlock
INSERT INTO Translations VALUES ('Guadagno del sensore del diaframma:','7','133')  -- DiaphragmSensorGainTextBlock

INSERT INTO Translations VALUES ('IMPOSTAZIONI DI SISTEMA','7','134')  -- SystemSettingsLabel
INSERT INTO Translations VALUES ('Gonfia velocità','7','135')  -- InflateSpeedTextBlock
INSERT INTO Translations VALUES ('Veloce','7','136')  -- FastTextBlock
INSERT INTO Translations VALUES ('Lento','7','137')  -- SlowTextBox
INSERT INTO Translations VALUES ('Sopra','7','138')  -- OnTextBlock
INSERT INTO Translations VALUES ('Via','7','139')  -- OffTextBlock
INSERT INTO Translations VALUES ('DMS','7','140')  -- DMSTextBlock
INSERT INTO Translations VALUES ('Linea','7','141')  -- LineTextBlock
INSERT INTO Translations VALUES ('La zona','7','142')  -- AreaTextBlock
INSERT INTO Translations VALUES ('Livello del refrigerante','7','143')  -- RefrigerantLevelTextBlock
INSERT INTO Translations VALUES ('Lbs','7','144')  -- LbsTextBlock
INSERT INTO Translations VALUES ('Min','7','145')  -- MinTextBlock

-- *****************************************************************************************
-- LOGIN SCREEN 
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Accesso','7','146')  -- LoginLabel
INSERT INTO Translations VALUES ('Username o password errati, per favore riprova!','7','147')  -- WrondUsernameOrPasswordLabel
INSERT INTO Translations VALUES ('OK','7','148')  -- OkButton

-- *****************************************************************************************
-- PATIENT SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Informazioni per il Paziente','7','149')  -- PatientInformationLabel
INSERT INTO Translations VALUES ('ID del paziente','7','150')  -- PatientIdLabel
INSERT INTO Translations VALUES ('Nome di battesimo','7','151')  -- FirstNameLabel
INSERT INTO Translations VALUES ('Cognome','7','152')  -- LastNameLabel
INSERT INTO Translations VALUES ('Sesso','7','153')  -- GenderLabel
INSERT INTO Translations VALUES ('Maschio','7','154')  -- MaleLabel
INSERT INTO Translations VALUES ('Femmina','7','155')  -- FemaleLabel
INSERT INTO Translations VALUES ('Data di nascita','7','156')  -- BirthDateLabel
INSERT INTO Translations VALUES ('DD','7','157')  -- DayLabel
INSERT INTO Translations VALUES ('MM','7','158')  -- MonthLabel
INSERT INTO Translations VALUES ('YYYY','7','159')  -- YearLabel
INSERT INTO Translations VALUES ('Medico','7','160')  -- PhysicianLabel

-- *****************************************************************************************
-- GENERIC SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Sì','7','161')  -- YESButton
INSERT INTO Translations VALUES ('NO','7','162')  -- NOButton
INSERT INTO Translations VALUES ('Sì','7','163')  -- YesButton
INSERT INTO Translations VALUES ('No','7','164')  -- NoButton
INSERT INTO Translations VALUES ('Sì','7','165')  -- YESLabel
INSERT INTO Translations VALUES ('NO','7','166')  -- NOLabel
INSERT INTO Translations VALUES ('Sì','7','167')  -- YesLabel
INSERT INTO Translations VALUES ('No','7','168')  -- NoLabel

-- *****************************************************************************************
-- MESSAGE POPUP SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('MESSAGGI DI SISTEMA','7','169')  -- SystemMessageLabel
INSERT INTO Translations VALUES ('MESSAGGIO DI AVVERTENZA','7','170')  -- WarningMessageLabel
INSERT INTO Translations VALUES ('MESSAGGIO DI ERRORE','7','171')  -- ErrorMessageLabel

-- *****************************************************************************************
-- SETTINGS MAIN SCREEN
-- *****************************************************************************************
INSERT INTO Translations VALUES ('Ora e data','7','172')  -- DateTimeLabel
INSERT INTO Translations VALUES ('Manuale utente','7','173')  -- UserManualLabel
INSERT INTO Translations VALUES ('Manutenzione','7','174')  -- MaintenanceLabel
INSERT INTO Translations VALUES ('Registro delle azioni','7','175')  -- ActionLogLabel
INSERT INTO Translations VALUES ('GIORNI','7','176')  -- DaysLabel
INSERT INTO Translations VALUES ('ORE','7','177')  -- HoursLabel
INSERT INTO Translations VALUES ('MINUTI','7','178')  -- MinutesLabel

-- *****************************************************************************************
-- Add new Translation here
-- *****************************************************************************************
INSERT INTO Translations VALUES ('THE DMS IS OFF','7','179')  -- NoPacingDetectedLabelOff
INSERT INTO Translations VALUES ('Weight','7','180')  -- WeightLabel
INSERT INTO Translations VALUES ('Height','7','181')  -- HeightLabel
INSERT INTO Translations VALUES ('Current Ablation Timer:','7','182')  -- ActualAblationTimerTextBlock
INSERT INTO Translations VALUES ('IF Vein Isolation Time >','7','183')  -- ExpectedVeinIsolationTimeTextBlock
INSERT INTO Translations VALUES ('THEN Set Ablation Timer =','7','184')  -- NewAblationTimerTextBlock
INSERT INTO Translations VALUES ('VEIN ISOLATION LOGIC','7','185')  -- VeinIsolationLogicLabel
INSERT INTO Translations VALUES ('Aggiornamento della durata dell isolamento della vena','7','186')  -- UpdateVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Nuova durata','7','187')  -- NewVeinIsolationDurationLabel
INSERT INTO Translations VALUES ('Durata non valida!','7','188')  -- InvalidDurationLabel
INSERT INTO Translations VALUES ('Timer di Ablazione','7','189')  -- AblationTimersLabel
INSERT INTO Translations VALUES ('Timer Fisso:','7','190')  -- FixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI Timer Fisso:','7','191')  -- TTIFixedTimerTextBlock
INSERT INTO Translations VALUES ('TTI + Timer di Durata:','7','192')  -- TTIDurationTimerTextBlock
INSERT INTO Translations VALUES ('Timer di Ablazione:  TTI + ','7','193')  -- AblationTimerTTIPlusTextBlock

INSERT INTO Translations VALUES ('Temperatura','7','194')  -- CryoBalloonTemperatureLabel
INSERT INTO Translations VALUES ('Si prega di selezionare un sito di ablazione','7','195')  -- SelectAblationSiteLabel
INSERT INTO Translations VALUES ('Esci dalla riproduzione','7','196')  -- ExitPlaybackButton
INSERT INTO Translations VALUES ('Modalità di riproduzione','7','197')  -- PlaybackModeLabel
INSERT INTO Translations VALUES ('Procedura completa','7','198')  -- CompleteProcedureButton
INSERT INTO Translations VALUES ('Ritorna alla procedura','7','199')  -- ReturnToProcedureButton
INSERT INTO Translations VALUES ('Fine della procedura','7','200')  -- EndProcedureButton
INSERT INTO Translations VALUES ('Riproduzione','7','201')  -- PlaybackButton
INSERT INTO Translations VALUES ('Data del caso','7','202')  -- CaseDateLabel
INSERT INTO Translations VALUES ('SALVA IN USB DRIVE','7','203')  -- SaveToUSBDriveLabel
INSERT INTO Translations VALUES ('Etichetta di volume:','7','204')  -- VolumeLabel
INSERT INTO Translations VALUES ('Nome:','7','205')  -- NameLabel
INSERT INTO Translations VALUES ('Formato guida:','7','206')  -- DriveFormatLabel
INSERT INTO Translations VALUES ('Spazio libero (byte):','7','207')  -- FreeSpaceLabel
INSERT INTO Translations VALUES ('Dimensione totale (byte):','7','208')  -- TotalSizeLabel
INSERT INTO Translations VALUES ('Nome del file:','7','209')  -- FileNameLabel
INSERT INTO Translations VALUES ('Tipo di file:','7','210')  -- FileTypeLabel
INSERT INTO Translations VALUES ('Torna al record di trattamento','7','211')  -- BackToTreatmentRecordButton
INSERT INTO Translations VALUES ('Utente','7','212')  -- UserLabel
INSERT INTO Translations VALUES ('Casa','7','213')  -- HomeLabel
INSERT INTO Translations VALUES ('Tipo di accesso','7','214')  -- AccessTypeLabel
INSERT INTO Translations VALUES ('Poi','7','215')  -- ThenTextBlock
INSERT INTO Translations VALUES ('Altro','7','216')  -- ElseTextBlock
INSERT INTO Translations VALUES ('Seleziona una Data ...','7','217')  -- SelectADateLabel
INSERT INTO Translations VALUES ('Solo persone qualificate','7','218')  -- QualifiedPersonLabel
INSERT INTO Translations VALUES ('Messaggio','7','219')  -- MessageLabel
INSERT INTO Translations VALUES ('Ripristina il Sistema','7','220')  -- ResetSystemButton
INSERT INTO Translations VALUES ('Vicino','7','221')  -- CloseButton
INSERT INTO Translations VALUES ('Azione Richiesta','7','222')  -- ActionRequiredLabel
INSERT INTO Translations VALUES ('Durata dell aggiornamento','7','223')  -- UpdateDurationLabel
INSERT INTO Translations VALUES ('Nuova durata','7','224')  -- NewDurationLabel
INSERT INTO Translations VALUES ('Qualsiasi modifica al sito di ablazione verrà salvata','7','225')  -- UpdateAblationSiteWarningLabel
INSERT INTO Translations VALUES ('Più file selezionati. Saranno salvati in:','7','226') -- MultipleFilesSelectedLabel
INSERT INTO Translations VALUES ('Errore','7','227') -- ErrorLabel
INSERT INTO Translations VALUES ('Non valido! ','7','228') -- InvalidDMSThresholdLabel
INSERT INTO Translations VALUES ('Min: ','7','229') -- MinLabel
INSERT INTO Translations VALUES ('Max: ','7','230') -- MaxLabel
INSERT INTO Translations VALUES ('Avviso audio','7','231') -- AudioAlertTextBlock
-- *****************************************************************************************
-- ITALIAN TRANSLATION - END
-- *****************************************************************************************


-- *****************************************************************************************
-- Languages 
-- *****************************************************************************************

-- Delete Languages
DELETE FROM Languages
DBCC CHECKIDENT (Languages, RESEED, 0)

-- *****************************************************************************************
-- Languages- BEGIN

INSERT INTO Languages VALUES ('English') -- ID 1
INSERT INTO Languages VALUES ('German') -- ID 2
INSERT INTO Languages VALUES ('French') -- ID 3
INSERT INTO Languages VALUES ('Japanese') -- ID 4
INSERT INTO Languages VALUES ('Danish') -- ID 5
INSERT INTO Languages VALUES ('Spanish') -- ID 6
INSERT INTO Languages VALUES ('Italian') -- ID 7
INSERT INTO Languages VALUES ('Dutch') -- ID 8
INSERT INTO Languages VALUES ('Norwegian') -- ID 9
INSERT INTO Languages VALUES ('Finnish') -- ID 10
INSERT INTO Languages VALUES ('Swedish') -- ID 11
INSERT INTO Languages VALUES ('Czech') -- ID 12
INSERT INTO Languages VALUES ('Hungarian') -- ID 13
INSERT INTO Languages VALUES ('Polish') -- ID 14
INSERT INTO Languages VALUES ('Romanian') -- ID 15
INSERT INTO Languages VALUES ('Croatian') -- ID 16
INSERT INTO Languages VALUES ('Slovak') -- ID 17
INSERT INTO Languages VALUES ('Slovene') -- ID 18
INSERT INTO Languages VALUES ('Greek') -- ID 19
INSERT INTO Languages VALUES ('Bulgarian') -- ID 20
INSERT INTO Languages VALUES ('Russian') -- ID 21
INSERT INTO Languages VALUES ('Turkish') -- ID 22
INSERT INTO Languages VALUES ('British English') -- ID 23
INSERT INTO Languages VALUES ('Estonian') -- ID 24
INSERT INTO Languages VALUES ('Latvian') -- ID 25
INSERT INTO Languages VALUES ('Lithuanian') -- ID 26
INSERT INTO Languages VALUES ('Brazilian') -- ID 27
INSERT INTO Languages VALUES ('Traditional Chinese') -- ID 28
INSERT INTO Languages VALUES ('Korean') -- ID 29
INSERT INTO Languages VALUES ('Simplified Chinese') -- ID 30
INSERT INTO Languages VALUES ('Arabic') -- ID 31
INSERT INTO Languages VALUES ('Thai') -- ID 32
INSERT INTO Languages VALUES ('Tamazight') -- ID 32

-- *****************************************************************************************
-- Errors 
-- *****************************************************************************************

-- Delete all table errors
DELETE FROM [ErrorMessages] 
DBCC CHECKIDENT ([ErrorMessages] , RESEED, 0)

DELETE FROM [Errors]  
DBCC CHECKIDENT ([Errors]  , RESEED, 0)

DELETE FROM [ErrorTypes]   
DBCC CHECKIDENT ([ErrorTypes]   , RESEED, 0)

-- *****************************************************************************************
-- errors- BEGIN


SET IDENTITY_INSERT [dbo].[Errors] ON 

INSERT [dbo].[Errors] ([Id], [Code]) VALUES (1, 1)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (2, 2)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (3, 4)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (4, 8)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (5, 16)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (6, 32)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (7, 64)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (8, 128)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (9, 256)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (10, 512)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (11, 1024)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (12, 2048)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (13, 4096)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (14, 8192)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (15, 16384)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (16, 32768)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (17, 65536)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (18, 131072)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (19, 262144)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (20, 524288)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (21, 1048576)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (22, 2097152)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (23, 4194304)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (26, 8388608)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (27, 16777216)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (24, 33554432)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (25, 67108864)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (28, 134217728)
-- *****************************************************************************************
-- GUI Messages errors
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (29, 26081)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (30, 26082)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (31, 26083)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (32, 26084)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (33, 26085)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (34, 26086)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (35, 26087)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (36, 26088)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (37, 26089)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (38, 260810)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (39, 260811)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (40, 260812)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (41, 260813)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (42, 260814)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (43, 260815)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (44, 260816)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (45, 260817)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (46, 260818)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (47, 260819)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (48, 260820)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (49, 260821)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (50, 260822)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (51, 260823)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (52, 260824)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (53, 260825)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (54, 260826)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (55, 260827)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (56, 260828)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (57, 260829)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (58, 260830)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (59, 260831)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (60, 260832)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (61, 260833)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (62, 260834)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (63, 260835)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (64, 260836)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (65, 260837)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (66, 260838)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (67, 260839)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (68, 260840)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (69, 260841)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (70, 260842)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (71, 260843)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (72, 260844)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (73, 260845)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (74, 260846)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (75, 260847)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (76, 260848)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (77, 260849)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (78, 260850)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (79, 260851)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (80, 260852)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (81, 260853)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (82, 260854)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (83, 260855)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (84, 260856)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (85, 260857)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (86, 260858)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (87, 260859)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (88, 260860)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (89, 260861)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (90, 260862)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (91, 260863)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (92, 260864)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (93, 260865)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (94, 260866)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (95, 260867)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (96, 260868)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (97, 260869)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (98, 260870)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (99, 260871)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (100, 260872)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (101, 260873)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (102, 260874)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (103, 260875)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (104, 260876)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (105, 260877)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (106, 260878)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (107, 260879)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (108, 260880)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (109, 260881)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (110, 260882)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (111, 260883)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (112, 260884)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (113, 260885)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (114, 260886)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (115, 260887)
INSERT [dbo].[Errors] ([Id], [Code]) VALUES (116, 260888)




SET IDENTITY_INSERT [dbo].[Errors] OFF

SET IDENTITY_INSERT [dbo].[ErrorMessages] ON 
-- *****************************************************************************************
-- ENGLISH TRANSLATION - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (1, 1, N'System has detected a hardware problem.', 'Report the System Notice number to Cryterion Medical technical support.', 1, 1, N'CPLD Watch Dog Timer Error')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (2, 1, N'System has detected a hardware problem.', 'Report the System Notice number to Cryterion Medical technical support.', 1, 2,  N'CMCU Two Multiplex Reading Does Not Match')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (3, 1, N'High refrigerant flow detected', 'Disconnect and reconnect cryo umbilical and try again. If the problem persists, replace thecryo umbilical then catheter. If problem persists, contact Cryterion Medical technical support.', 1, 4, N'Error 2- 00000004 High refrigerant flow detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (4, 1, N'Refrigerant flow obstruction detected', 'Disconnect and reconnect cryo umbilical and try again. If the problem persists, replace the cryo umbilical then catheter. If problem persists, contact Cryterion Medical technical support.', 1, 8, N'Error 2- 00000008 Refrigerant flow obstruction detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (6, 1, N'Catheter Disconnected When Vacuum Applied', 'Solution', 1, 16, N'Error 2- 00000010 Catheter Disconnected When Vacuum Applied')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (7, 1, N'Low Refrigerant level in tank', 'Must change tank before the next procedure', 1, 32, N'Warning- 00000020 Low Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (8, 1, N'Insufficient Refrigerant level in tank', 'Replace the refrigerant tank', 1, 64, N'Error 2- 00000040 Insufficient Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (9, 1, N'Catheter is disconnected during treatment', 'Solution', 1, 128, N'Error 2- 00000080 Catheter is disconnected during treatment')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (10, 1, N'Tank pressure is too high', 'Solution', 1, 256, N'Warning- 00000100 Tank pressure is too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (11, 1, N'Tank pressure is too low.Please open tank or replace.', 'Ensure refrigerant tank is open', 1, 512, N'Warning- 00000200 Tank pressure is too low.Please open tank or replace.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (12, 1, N'Tank pressure is too high.Please open tank door.', 'Check if tank enclosure fans are working. Shut down the system and wait 10 minutes before restarting. If problem persists, contact Cryterion Medical technical support.', 1, 1024, N'Error 2- 00000400 Tank pressure is too high.Please open tank door.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (13, 1, N'System timeout', 'Solution', 1, 2048, N'Error 2- 00000800 GUI Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (14, 1, N'Injection pressure too high', 'Disconnect the cryo umbilical from the console and make sure there are no obstructions in the console cryo-port. If problem persists, contact Cryterion Medical technical support.', 1, 4096, N'Error 2- 00001000 Injection pressure too high.(PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (15, 1, N'Critical system error.', 'Solution', 1, 8192, N'Error 2- Critical system error')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (16, 1, N'Obstruction detected. Return pressure too high', 'Disconnect and reconnect the cryo-umbilical cable. Try again before replacing the catheter. If problem persists, contact Cryterion Medical technical support.', 1, 16384, N'Error 2- 00004000 Obstruction detected. Return pressure too high. (PT3)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (17, 1, N'Firmware timeout', 'Solution', 1, 32768, N'Error 2- 00008000 -	Control Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (18, 1, N'Obstruction detected. Insufficient vacuum level', 'Disconnect and reconnect cryoumbilical and try again. If the problem persists, replace the cryo-umbilical then catheter. If problem persists, contact Cryterion Medical technical support.', 1, 65536, N'Error 2- 00010000 Obstruction detected. Insufficient vacuum level. (PT4)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (19, 1, N'Firmware timeout', 'Solution', 1, 131072, N'Error 2- 00020000 -	Patient Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20, 1, N'Subcooler temperature too high', 'Wait 10 minutes and try again. If problem persists contact Cryterion Medical technical support.', 1, 262144, N'Warning- 00040000 Subcooler temperature too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (21, 1, N'Subcooler temperature out of range', 'Solution', 1, 524288, N'Error 2- 00080000 Subcooler temperature out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (22, 1, N'Venting line error detected', 'Wait 10 minutes and try again. If problem persists contact Cryterion Medical technical support.', 1, 1048576, N'Error 2- 00100000 Venting line error detected. (PS1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (23, 1, N'The system has detected a stuck input  button (start stop or foot switch )', 'Solution', 1, 2097152, N'Warning- 00200000 The system has detected a stuck input  button (start stop or foot switch )')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (24, 1, N'Scavenging line pressure too high', 'Ensure that hospital scavenging system is turned on and the scavenging hose is securely attached.', 1, 4194304, N'Error 2- 00400000 Scavenging line pressure too high(PT5)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (25, 1, N'Catheter Tube Connected', 'Solution', 1, 33554432, N'Catheter Tube Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (26, 1, N'System has detected a hardware problem.', 'Solution', 1, 67108864, N'Error 2- 04000000 CMCU Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (27, 1, N'Scavenging line pressure reading out of range', 'Solution', 1, 8388608, N'Error 2- 00800000 Scavenging line pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (28, 1, N'Vein Isolated', 'Solution', 1, 16777216, N'Vein Isolated')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (29, 1, N'CMCU Ready', 'Solution', 1, 134217728, N'CMCU Ready')


INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30, 1, N'System has detected a hardware problem', 'Report the System Notice number to Cryterion Medical technical support.', 2, 1, N'Error 1- 00000001 Hardware Error – CPLD WDT')

INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (46, 1, N'System has detected a hardware problem.', 'Solution', 2, 2, N'Error 2- 00000002 Pateint Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (31, 1, N'Inner balloon pressure too high', 'Continue the procedure. If the problem persists, replace the catheter.', 2, 4, N'Error 1- 00000004  Inner balloon pressure too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (32, 1, N'Inner balloon pressure too low', 'Repeat the inflation, if the problem persists replace the catheter', 2, 8, N'Error 1- 00000008  Inner balloon pressure too low')

INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (33, 1, N'Inner balloon pressure too low', 'Solution', 2, 8, N'Error 1- 00000008 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (34, 1, N'Outer balloon breach detected', 'Replace the catheter', 2, 32, N'Error 1- 00000020 Outer balloon breach detected')
--INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (35, 1, N'Outer Balloon Pressure Too High', 'Solution', 2, 32, N'Error 1- 00000020 Outer Balloon Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (36, 1, N'Outer balloon pressure reading out of range', 'Solution', 2, 64, N'Error 1- 00000040 Outer balloon pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (37, 1, N'Balloon Tip Pressure Too High', 'Solution', 2, 128, N'Error 1- 00000080 Balloon Tip Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (38, 1, N'Balloon Tip Pressure Too Low', 'Solution', 2, 256, N'Error 1- 00000100 Balloon Tip Pressure Too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (39, 1, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range', 'Solution', 2, 512, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (40, 1, N'Thawing Temperature Too High', 'Solution', 2, 1024, N'Error 1- 00000400 Thawing Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (41, 1, N'Balloon temperature too Low', 'Solution', 2, 2048, N'Error 1- 00000800 Balloon temperature too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (42, 1, N'Balloon Temperature Too High', 'Solution', 2, 4096, N'Error 1- 0001000 Balloon Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (43, 1, N'Blood detected in the catheter. Please replace the catheter', 'Solution', 2, 16384, N'Error 1- 0004000 Blood detected in the catheter. Please replace the catheter')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (45, 1, N'Catheter Cable Connected', 'Solution', 2, 16777216, N'Catheter Cable Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message],[SolutionMessage],  [Type], [ErrorCode], [CryterionMessage]) VALUES (48, 1, N'PMCU Ready', 'Solution', 2, 134217728, N'PMCU Ready')
-- *****************************************************************************************
-- GUI MESSAGES - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (49, 1, N'Load cell error', 'Solution', 3, 26081, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (50, 1, N'Do you want to reset', 'Solution', 3, 26082, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (51, 1, N'System Error', 'Solution', 3, 26083, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (52, 1, N'Ablation Writing Error', 'Solution', 3, 26084, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (53, 1, N'Ablation ECG Writing Error', 'Solution', 3, 26085, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (54, 1, N'Stop wrinting to JSON file', 'Solution', 3, 26086, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (55, 1, N'Treatment Loading Error', 'Solution', 3, 26087, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (56, 1, N'Please Select A Registetr', 'Solution', 3, 26088, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (57, 1, N'Are you sure that you want to close the software?', 'Solution', 3, 26089, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (58, 1, N'Are you sure that you want to shutdown your computer now?', 'Solution', 3, 260810, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (59, 1, N'Are you sure you want to end the procedure ?', 'Solution', 3, 260811, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60, 1, N'You do not have sufficient privileges to access the Settings.', 'Solution', 3, 260812, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (61, 1, N'Are you sure you want to end the procedure without adding any out come notes?', 'Solution', 3, 260813, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (62, 1, N'Are you sure you want to quit the procedure?', 'Solution', 3, 260814, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (63, 1, N'Are you sure you want to logout from the system?', 'Solution', 3, 260815, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (64, 1, N'The username :', 'Solution', 3, 260816, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (65, 1, N'already exists!', 'Solution', 3, 260817, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (66, 1, N'UserName Exists', 'Solution', 3, 260818, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (67, 1, N'already exists but the user is inactive.  Do you want to reactivate it ?', 'Solution', 3, 260819, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (68, 1, N'Re-Activate User ?', 'Solution', 3, 260820, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (69, 1, N'The physician name :', 'Solution', 3, 260821, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70, 1, N'Physician Exists', 'Solution', 3, 260822, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (71, 1, N'Do you really want to delete the user :', 'Solution', 3, 260823, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (72, 1, N'Delete User', 'Solution', 3, 260824, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (73, 1, N'The current password for :', 'Solution', 3, 260825, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (74, 1, N'is not valid!', 'Solution', 3, 260826, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (75, 1, N'Password Invalid', 'Solution', 3, 260827, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (76, 1, N'Could not retrieve the selected Physician!', 'Solution', 3, 260828, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (77, 1, N'Physician not found', 'Solution', 3, 260829, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (78, 1, N'A physician must be selected!', 'Solution', 3, 260830, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (79, 1, N'Physician Missing', 'Solution', 3, 260831, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (80, 1, N'The patient s birth date is not valid!', 'Solution', 3, 260832, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (81, 1, N'Invalid Date', 'Solution', 3, 260833, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (82, 1, N'This patient ID already exists in the database!', 'Solution', 3, 260834, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (83, 1, N'Patient Already Exist', 'Solution', 3, 260835, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (84, 1, N'An error occurred while inserting a new Patient in the database!', 'Solution', 3, 260836, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (85, 1, N'Patient Insertion Error', 'Solution', 3, 260837, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (86, 1, N'The Physician could not be retrieved!', 'Solution', 3, 260838, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (87, 1, N'An error occurred while creating the ablation procedure!', 'Solution', 3, 260839, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (88, 1, N'Procedure Creation Error', 'Solution', 3, 260840, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (89, 1, N'An error occurred while generating the USB drive list!', 'Solution', 3, 260841, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (90, 1, N'USB Drive List error', 'Solution', 3, 260842, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (91, 1, N'The specified path is invalid or cannot be found!', 'Solution', 3, 260843, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (92, 1, N'Engineering Data Not Saved!', 'Solution', 3, 260844, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (93, 1, N'Access denied.  You don t have access to the specified path!', 'Solution', 3, 260845, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (94, 1, N'The specified path is invalid!', 'Solution', 3, 260846, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (95, 1, N'The specified path is invalid!  An unsupported character has been detected.', 'Solution', 3, 260847, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (96, 1, N'Target file or directory does not exist anymore!', 'Solution', 3, 260848, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (97, 1, N'An error occurred while saving the engineering data files to the USB drive!', 'Solution', 3, 260849, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (98, 1, N'An error occurred while saving the engineering data files to the USB drive!', 'Solution', 3, 260850, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (99, 1, N'The engineering data files have been saved to USB drive successfully!', 'Solution', 3, 260851, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (100, 1, N'Engineering Data Saved Successfully!', 'Solution', 3, 260852, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (101, 1, N'An error occurred while generating the USB drive list!', 'Solution', 3, 260853, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (102, 1, N'USB Drive List error', 'Solution', 3, 260854, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (103, 1, N'An error occurred while saving the procedure s outcome to the database!', 'Solution', 3, 260855, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (104, 1, N'Error Saving Outcome', 'Solution', 3, 260856, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (105, 1, N'An error occurred while saving the procedure s diagnosis to the database!', 'Solution', 3, 260857, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (106, 1, N'Error Saving Diagnosis', 'Solution', 3, 260858, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (107, 1, N'The hospital informations are not valid', 'Solution', 3, 260859, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (108, 1, N'An error occurred while generating the USB drive list!', 'Solution', 3, 260860, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (109, 1, N'USB Drive List error', 'Solution', 3, 260861, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (110, 1, N'An error occurred while generating the procedure records list!', 'Solution', 3, 260862, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (111, 1, N'Procedure Records Error', 'Solution', 3, 260863, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (112, 1, N'The procedure has been saved to USB drive successfully!', 'Solution', 3, 260864, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (113, 1, N'Procedure Saved Successfully!', 'Solution', 3, 260865, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (114, 1, N'The specified path is invalid or cannot be found!', 'Solution', 3, 260866, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (115, 1, N'Procedure Not Saved!', 'Solution', 3, 260867, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (116, 1, N'Access denied.  You don t have access to the specified path!', 'Solution', 3, 260868, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (117, 1, N'The specified path is invalid!', 'Solution', 3, 260869, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (118, 1, N'The specified path is invalid!  An unsupported character has been detected.', 'Solution', 3, 260870, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (119, 1, N'Target file or directory does not exist anymore!', 'Solution', 3, 260871, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (120, 1, N'An error occurred while saving the procedure to the USB drive!', 'Solution', 3, 260872, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (121, 1, N'Do you really want to clear the warning messages list ?', 'Solution', 3, 260873, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (122, 1, N'Clear System Notification List', 'Solution', 3, 260874, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (123, 1, N'An error occurred while updating tip/balloon pressure charts!', 'Solution', 3, 260875, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (124, 1, N'Tip/Balloon Pressure Chart Error', 'Solution', 3, 260876, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (125, 1, N'An error occurred while Loading on charts!', 'Solution', 3, 260877, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (126, 1, N'Temperature/Diaphragm Movement Chart Error', 'Solution', 3, 260878, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (127, 1, N'An error occured while trying to display the treatment notes.', 'Solution', 3, 260879, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (128, 1, N'Treatment Notes Error', 'Solution', 3, 260880, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (129, 1, N'An error occurred during ablation', 'Solution', 3, 260881, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (130, 1, N'CAN1 Communication', 'Solution', 3, 260882, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (131, 1, N'CAN2 Communication', 'Solution', 3, 260883, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (132, 1, N'This language is not supported in this version yet.  Please select another one.', 'Solution', 3, 260884, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (133, 1, N'Please restart the system to apply new language settings.', 'Solution', 3, 260885, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (134, 1, N'You don''t have sufficient privileges to access the Records.', 'Solution', 3, 260886, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (135, 1, N'DAS Balloon Error', 'Press DAS balloon Button to set the right pressure and start the ablation', 3, 260887, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (136, 1, N'DAS Balloon Error', 'Press DAS balloon Button to set the right pressure and start the ablation', 3, 260888, 'N/A')

-- *****************************************************************************************
-- ENGLISH TRANSLATION - END
-- *****************************************************************************************

-- *****************************************************************************************
-- GERMAN TRANSLATION - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20001, 2, N'Die Konsole hat ein Hardwareproblem entdeckt.', 'Verbindung der ICB zur Konsole unterbrechen und Konsole neu hochfahren. Sobald die Konsole wieder hochgefahren ist, die ICB mit der Konsole verbinden. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 1, N'CPLD Watch Dog Timer Error')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20002, 2, N'Die Konsole hat ein Hardwareproblem entdeckt.', 'Verbindung der ICB zur Konsole unterbrechen und Konsole neu hochfahren. Sobald die Konsole wieder hochgefahren ist, die ICB mit der Konsole verbinden. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 2,  N'CMCU Two Multiplex Reading Does Not Match')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20003, 2, N'Hoher Kühlmittelfluss entdeckt', 'Kryokabel aus- und wieder einstecken und eine weitere Ablation versuchen. Falls das Problem weiterhin auftritt, erst das Kryokabel und dann den Katheter austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 4, N'Error 2- 00000004 High refrigerant flow detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20004, 2, N'Blockierung des Kühlmittelflusses entdeckt', 'Das Kryokabel aus- und wieder einstecken und eine weitere Ablation versuchen. Falls das Problem weiterhin auftritt, erst das Kryokabel und dann den Katheter austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 8, N'Error 2- 00000008 Refrigerant flow obstruction detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20006, 2, N'Der Katheter wurde während des Anlegens von Vakuum mechanisch gelöst', 'Achten Sie darauf, dass das Kryokabel ordentlich mit der Konsole und dem Katheter verbunden ist. Falls das Problem weiterhin auftritt, das Kryokabel und dann den Katheter wechseln. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 16, N'Error 2- 00000010 Catheter Disconnected When Vacuum Applied')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20007, 2, N'Niedriger Kühlmittelstand im Tank', 'Überprüfen Sie die verbleibende angezeigte Ablationszeit, um sicherzugehen, dass ausreichend Kühlmittel vorhanden ist, um den Vorgang zu beenden. 

Falls nötig Tank austauschen.', 1, 32, N'Warning- 00000020 Low Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20008, 2, N'Kühlmittelstand im Tank nicht ausreichend für Durchführung eines Verfahrens', 'Kühlmitteltank austauschen', 1, 64, N'Error 2- 00000040 Insufficient Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (20009, 2, N'Die Konsole hat festgestellt, dass der Katheter während der Behandlung elektrisch gelöst wurde', 'Verbindung der ICB zur Konsole unterbrechen und wieder herstellen. Falls das Problem weiterhin auftritt, das elektrische Kabel des Katheters von der ICB und dann dem Katheter lösen und wieder verbinden. Vakuum anlegen, um fortzufahren. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 128, N'Error 2- 00000080 Catheter is disconnected during treatment')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200010, 2, N'Tank pressure is too high', 'Solution', 1, 256, N'Warning- 00000100 Tank pressure is too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200011, 2, N'Der Druck im Tank ist zu niedrig.', 'Sicherstellen, dass das Ventil des Kühlmitteltanks offen ist. Falls das Problem weiterhin auftritt, den Tank austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 512, N'Warning- 00000200 Tank pressure is too low.Please open tank or replace.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200012, 2, N'Der Druck im Tank ist zu hoch.', 'Sicherstellen, dass die Konsolenlüfter funktionieren. Die Tanktüren öffnen und die Konsole abschalten. Falls die Konsolenlüfter funktionierten, vor dem Neustart mindestens 10 Minuten warten. Ansonsten, falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 1024, N'Error 2- 00000400 Tank pressure is too high.Please open tank door.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200013, 2, N'Die Konsole hat ein Softwareproblem entdeckt', 'Verbindung der ICB zur Konsole unterbrechen und Konsole neu hochfahren. Sobald die Konsole wieder hochgefahren ist, die ICB mit der Konsole verbinden. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 2048, N'Error 2- 00000800 GUI Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200014, 2, N'Der Einspritzdruck ist zu hoch', 'Kryokabel austauschen und eine weitere Ablation versuchen. Falls das Problem weiterhin auftritt, den Katheter austauschen. 
Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 4096, N'Error 2- 00001000 Injection pressure too high.(PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200015, 2, N'Die Konsole hat ein Hardwareproblem entdeckt', 'Den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 8192, N'Error 2- 00002000 Injection pressure reading out of range. (PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200016, 2, N'Blockierung des Durchflusses entdeckt', 'Das Kryokabel aus- und wieder
einstecken. Falls das Problem weiterhin auftritt, den Katheter austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 16384, N'Error 2- 00004000 Obstruction detected. Return pressure too high. (PT3)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200017, 2, N'Die Konsole hat ein Hardwareproblem entdeckt', 'Verbindung der ICB zur Konsole unterbrechen und Konsole neu hochfahren. Sobald die Konsole wieder hochgefahren ist, die ICB mit der Konsole verbinden. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 32768, N'Error 2- 00008000 -	Control Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200018, 2, N'Blockierung des Durchflusses entdeckt', 'Eine weitere Ablation versuchen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 65536, N'Error 2- 00010000 Obstruction detected. Insufficient vacuum level. (PT4)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200019, 2, N'Die Konsole hat ein Hardwareproblem entdeckt', 'Verbindung der ICB zur Konsole unterbrechen und Konsole neu hochfahren. Sobald die Konsole wieder hochgefahren ist, die ICB mit der Konsole verbinden. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 131072, N'Error 2- 00020000 -	Patient Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200020, 2, N'Die Temperatur des Unterkühlers ist zu hoch', 'Vor dem nächsten Ablationsversuch 5 Minuten warten. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 262144, N'Warning- 00040000 Subcooler temperature too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200021, 2, N'Subcooler temperature out of range', 'Solution', 1, 524288, N'Error 2- 00080000 Subcooler temperature out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200022, 2, N'Die Konsole hat ein Hardwareproblem entdeckt', 'Vor dem nächsten Ablationsversuch 5 Minuten warten. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 1048576, N'Error 2- 00100000 Venting line error detected. (PS1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200023, 2, N'Venting line error out of range', 'Solution', 1, 2097152, N'Error 2- 00200000 Venting line error out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200024, 2, N'Der Druck in der Spülleitung ist zu hoch', 'Sicherstellen, dass das Spülsystem
des Krankenhauses aktiviert und der Spülschlauch sicher befestigt ist. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 4194304, N'Error 2- 00400000 Scavenging line pressure too high(PT5)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200025, 2, N'Catheter Tube Connected', 'Solution', 1, 33554432, N'Catheter Tube Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200026, 2, N'Die Konsole hat den Selbsttest nicht bestanden.', 'Konsole neu hochfahren. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 1, 67108864, N'Error 2- 04000000 CMCU Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200027, 2, N'Scavenging line pressure reading out of range', 'Solution', 1, 8388608, N'Error 2- 00800000 Scavenging line pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200028, 2, N'Vein Isolated', 'Solution', 1, 16777216, N'Vein Isolated')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200029, 2, N'CMCU Ready', 'Solution', 1, 134217728, N'CMCU Ready')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200030, 2, N'System has detected a hardware problem', 'Report the System Notice number to Cryterion Medical technical support.', 2, 2, N'Error 1- 00000001 Hardware Error – CPLD WDT')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200031, 2, N'Die Konsole hat den Selbsttest nicht bestanden', 'Konsole neu hochfahren. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 2, 2, N'Error 1- 00000002 Inner balloon pressure too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200032, 2, N'Der Innenballondruck ist zu hoch', 'Eine weitere Ablation versuchen. Falls das Problem weiterhin auftritt, erst das Kryokabel und dann den Katheter austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 2, 4, N'Error 1- 00000004 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200033, 2, N'Der Innenballondruck ist zu niedrig', 'Eine weitere Ablation versuchen. Falls das Problem weiterhin auftritt, den Katheter austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben', 2, 8, N'Error 1- 00000008 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200034, 2, N'Outer balloon breach detected', 'Replace the catheter', 2, 16, N'Error 1- 00000010 Outer balloon breach detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200035, 2, N'Der Außenballondruck ist zu hoch', 'Das Kryokabel an der Konsole und am Katheter aus- und wieder einstecken. Falls das Problem weiterhin auftritt, den Katheter und das Kryokabel austauschen. Falls das Problem weiterhin auftritt, den technischen Support von Cryterion Medical kontaktieren und den Fehlercode angeben.', 2, 32, N'Error 1- 00000020 Outer Balloon Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200036, 2, N'Outer balloon pressure reading out of range', 'Solution', 2, 64, N'Error 1- 00000040 Outer balloon pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200037, 2, N'Balloon Tip Pressure Too High', 'Solution', 2, 128, N'Error 1- 00000080 Balloon Tip Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200038, 2, N'Balloon Tip Pressure Too Low', 'Solution', 2, 256, N'Error 1- 00000100 Balloon Tip Pressure Too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200039, 2, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range', 'Solution', 2, 512, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200040, 2, N'Thawing Temperature Too High', 'Solution', 2, 1024, N'Error 1- 00000400 Thawing Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200041, 2, N'Balloon temperature too Low', 'Solution', 2, 2048, N'Error 1- 00000800 Balloon temperature too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200042, 2, N'Die Ballontemperatur ist zu niedrig. Der Katheter ist möglicherweise zu tief in der Vene', 'Den Katheter neu positionieren und eine weitere Ablation versuchen.', 2, 4096, N'Error 1- 0001000 Balloon Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200043, 2, N'Die Konsole hat Blut im Katheter entdeckt', 'Katheter austauschen. Mit diesem Katheter keine weiteren Aufblasversuche oder Ablationen durchführen.', 2, 16384, N'Error 1- 0004000 Blood detected in the catheter. Please replace the catheter')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200045, 2, N'Catheter Cable Connected', 'Solution', 2, 16777216, N'Catheter Cable Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200046, 2, N'System has detected a hardware problem.', 'Solution', 2, 67108864, N'Error 2- 04000000 Pateint Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message],[SolutionMessage],  [Type], [ErrorCode], [CryterionMessage]) VALUES (200048, 2, N'PMCU Ready', 'Solution', 2, 134217728, N'PMCU Ready')

-- *****************************************************************************************
-- GUI MESSAGES - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200049, 2, N'Wägezellenfehler', 'Solution', 3, 26081, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200050, 2, N'Möchten Sie zurücksetzen', 'Solution', 3, 26082, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200051, 2, N'Systemfehler', 'Solution', 3, 26083, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200052, 2, N'Ablations-Schreibfehler', 'Solution', 3, 26084, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200053, 2, N'Ablations-EKG-Schreibfehler', 'Solution', 3, 26085, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200054, 2, N'Schreiben auf JSON-Datei einstellen', 'Solution', 3, 26086, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200055, 2, N'Behandlungs-Ladefehler', 'Solution', 3, 26087, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200056, 2, N'Bitte ein Register auswählen', 'Solution', 3, 26088, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200057, 2, N'Sind Sie sicher, dass Sie die Software schließen möchten?', 'Solution', 3, 26089, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200058, 2, N'Sind Sie sicher, dass Sie Ihren Computer herunterfahren möchten?', 'Solution', 3, 260810, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200059, 2, N'Sind Sie sicher, dass Sie das Verfahren beenden möchten?', 'Solution', 3, 260811, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200060, 2, N'Sie haben keine ausreichenden Privilegien, um auf die Einstellungen zuzugreifen.', 'Solution', 3, 260812, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200061, 2, N'Sind Sie sicher, dass Sie das Verfahren beenden möchten, ohne Anmerkungen über den Ausgang hinzuzufügen?', 'Solution', 3, 260813, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200062, 2, N'Sind Sie sicher, dass Sie das Verfahren abbrechen möchten?', 'Solution', 3, 260814, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200063, 2, N'Sind Sie sicher, dass Sie sich aus dem System abmelden möchten?', 'Solution', 3, 260815, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200064, 2, N'Der Benutzername:', 'Solution', 3, 260816, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200065, 2, N'existiert bereits!', 'Solution', 3, 260817, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200066, 2, N'Benutzername existiert', 'Solution', 3, 260818, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200067, 2, N'existiert bereits, doch der Benutzer ist inaktiv.  Möchten Sie ihn reaktivieren ?', 'Solution', 3, 260819, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200068, 2, N'Benutzer reaktivieren?"', 'Solution', 3, 260820, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200069, 2, N'Der Arztname:', 'Solution', 3, 260821, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200070, 2, N'Arzt existiert', 'Solution', 3, 260822, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200071, 2, N'Möchten Sie den Benutzer wirklich löschen:', 'Solution', 3, 260823, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200072, 2, N'Benutzer löschen', 'Solution', 3, 260824, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200073, 2, N'Das aktuelle Passwort für:', 'Solution', 3, 260825, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200074, 2, N'ist ungültig!', 'Solution', 3, 260826, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200075, 2, N'Passwort ungültig', 'Solution', 3, 260827, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200076, 2, N'Der gewählte Arzt konnte nicht abgerufen werden!', 'Solution', 3, 260828, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200077, 2, N'Arzt nicht gefunden', 'Solution', 3, 260829, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200078, 2, N'Es muss ein Arzt ausgewählt werden!', 'Solution', 3, 260830, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200079, 2, N'Arzt fehlt', 'Solution', 3, 260831, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200080, 2, N'Das Geburtsdatum des Patienten ist ungültig!', 'Solution', 3, 260832, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200081, 2, N'Ungültiges Datum', 'Solution', 3, 260833, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200082, 2, N'Diese Patienten-ID existiert bereits in der Datenbank!', 'Solution', 3, 260834, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200083, 2, N'Patient existiert bereits', 'Solution', 3, 260835, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200084, 2, N'Beim Eintragen eines neuen Patienten in die Datenbank ist ein Fehler aufgetreten!', 'Solution', 3, 260836, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200085, 2, N'Fehler beim Eintragen des Patienten', 'Solution', 3, 260837, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200086, 2, N'Der Arzt konnte nicht abgerufen werden!', 'Solution', 3, 260838, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200087, 2, N'Beim Erstellen des Ablationsverfahrens ist ein Fehler aufgetreten!', 'Solution', 3, 260839, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200088, 2, N'Fehler beim Erstellen des Verfahrens', 'Solution', 3, 260840, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200089, 2, N'Beim Anlegen der USB-Stick-Liste ist ein Fehler aufgetreten!"', 'Solution', 3, 260841, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200090, 2, N'USB-Stick-Liste Fehler', 'Solution', 3, 260842, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200091, 2, N'Der angegebene Pfad ist ungültig oder kann nicht gefunden werden!', 'Solution', 3, 260843, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200092, 2, N'Engineering-Daten nicht gespeichert!', 'Solution', 3, 260844, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200093, 2, N'Zugriff verweigert.  Sie haben keinen Zugriff auf den angegebenen Pfad!', 'Solution', 3, 260845, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200094, 2, N'Der angegebene Pfad ist ungültig!', 'Solution', 3, 260846, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200095, 2, N'Der angegebene Pfad ist ungültig!  Es wurde ein unzulässiges Zeichen entdeckt.', 'Solution', 3, 260847, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200096, 2, N'Die Zieldatei oder das Zielverzeichnis existiert nicht mehr!', 'Solution', 3, 260848, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200097, 2, N'Beim Speichern der Engineering-Daten-Dateien auf dem USB-Stick ist ein Fehler aufgetreten!', 'Solution', 3, 260849, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200098, 2, N'Beim Speichern der Engineering-Daten-Dateien auf dem USB-Stick ist ein Fehler aufgetreten!', 'Solution', 3, 260850, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200099, 2, N'Die Engineering-Daten-Dateien wurden erfolgreich auf dem USB-Stick gespeichert!', 'Solution', 3, 260851, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200100, 2, N'Engineering-Daten erfolgreich gespeichert!', 'Solution', 3, 260852, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200101, 2, N'Beim Anlegen der USB-Stick-Liste ist ein Fehler aufgetreten!', 'Solution', 3, 260853, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200102, 2, N'USB-Stick-Liste Fehler', 'Solution', 3, 260854, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200103, 2, N'Beim Speichern des Ausgangs des Verfahrens in der Datenbank ist ein Fehler aufgetreten!', 'Solution', 3, 260855, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200104, 2, N'Fehler beim Speichern des Ausgangs', 'Solution', 3, 260856, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200105, 2, N'Beim Speichern der Diagnose des Verfahrens in der Datenbank ist ein Fehler aufgetreten!', 'Solution', 3, 260857, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200106, 2, N'Fehler beim Speichern der Diagnose', 'Solution', 3, 260858, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200107, 2, N'Die Krankenhausangaben sind ungültig', 'Solution', 3, 260859, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200108, 2, N'Beim Anlegen der USB-Stick-Liste ist ein Fehler aufgetreten!', 'Solution', 3, 260860, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200109, 2, N'USB-Stick-Liste Fehler', 'Solution', 3, 260861, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200110, 2, N'Beim Anlegen der Verfahrens-Aufzeichnungsliste ist ein Fehler aufgetreten!', 'Solution', 3, 260862, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200111, 2, N'Fehler bei Verfahrens-Aufzeichnungen', 'Solution', 3, 260863, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200112, 2, N'Das Verfahren wurde erfolgreich auf dem USB-Stick gespeichert!', 'Solution', 3, 260864, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200113, 2, N'Verfahren erfolgreich gespeichert!', 'Solution', 3, 260865, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200114, 2, N'Der angegebene Pfad ist ungültig oder kann nicht gefunden werden!', 'Solution', 3, 260866, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200115, 2, N'Verfahren nicht gespeichert!', 'Solution', 3, 260867, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200116, 2, N'Zugriff verweigert.  Sie haben keinen Zugriff auf den angegebenen Pfad!', 'Solution', 3, 260868, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200117, 2, N'Der angegebene Pfad ist ungültig!', 'Solution', 3, 260869, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200118, 2, N'Der angegebene Pfad ist ungültig!  Es wurde ein unzulässiges Zeichen entdeckt.', 'Solution', 3, 260870, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200119, 2, N'Die Zieldatei oder das Zielverzeichnis existiert nicht mehr!', 'Solution', 3, 260871, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200120, 2, N'Beim Speichern des Verfahrens auf dem USB-Stick ist ein Fehler aufgetreten!', 'Solution', 3, 260872, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200121, 2, N'Möchten Sie die Liste der Warnmeldungen wirklich löschen?', 'Solution', 3, 260873, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200122, 2, N'Liste der Warnmeldungen löschen', 'Solution', 3, 260874, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200123, 2, N'Beim Aktualisieren der Spitzen-/Ballondruck-Tabellen ist ein Fehler aufgetreten!', 'Solution', 3, 260875, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200124, 2, N'Fehler Spitzen-/Ballondruck-Tabelle', 'Solution', 3, 260876, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200125, 2, N'Beim Laden der Tabellen ist ein Fehler aufgetreten!"', 'Solution', 3, 260877, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200126, 2, N'Fehler Temperatur/Diaphragma-Bewegungs-Tabelle"', 'Solution', 3, 260878, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200127, 2, N'Beim Versuch, die Behandlungs-Anmerkungen anzuzeigen, ist ein Fehler aufgetreten.', 'Solution', 3, 260879, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200128, 2, N'Fehler Behandlungs-Anmerkungen', 'Solution', 3, 260880, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200129, 2, N'Während der Ablation ist ein Fehler aufgetreten', 'Solution', 3, 260881, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200130, 2, N'CAN1 Communication', 'Solution', 3, 260882, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200131, 2, N'CAN2 Communication', 'Solution', 3, 260883, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200132, 2, N'This language is not supported in this version yet.', 'Solution', 3, 260884, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (200133, 2, N'Please restart the system to apply new language settings.', 'Solution', 3, 260885, 'N/A')
-- *****************************************************************************************
-- GERMAN TRANSLATION - END
-- *****************************************************************************************

-- *****************************************************************************************
-- FRENCH TRANSLATION - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30001, 3, N'La console a détecté un problème matériel.', 'Débranchez l’ICB de la console et redémarrez la console. Une fois que la console aura redémarré, branchez l’ICB à la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 1, N'CPLD Watch Dog Timer Error')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30002, 3, N'La console a détecté un problème matériel.', 'Débranchez l’ICB de la console et redémarrez la console. Une fois que la console aura redémarré, branchez l’ICB à la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 2,  N'CMCU Two Multiplex Reading Does Not Match')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30003, 3, N'Débit de fluide frigorigène élevé détecté', 'Débranchez et rebranchez le cryocâble et essayez une autre ablation. Si le problème persiste, remplacez le cryocâble puis le cathéter. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 4, N'Error 2- 00000004 High refrigerant flow detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30004, 3, N'Obstruction du débit de fluide frigorigène détectée', 'Débranchez et rebranchez le cryocâble et essayez une autre ablation. Si le problème persiste, remplacez le cryocâble puis le cathéter. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 8, N'Error 2- 00000008 Refrigerant flow obstruction detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30006, 3, N'Le cathéter a été mécaniquement débranché lors de l’application du vide', 'Assurez-vous que le cryocâble est correctement branché à la console et au cathéter. Si le problème persiste, changez le cryocâble puis le cathéter. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 16, N'Error 2- 00000010 Catheter Disconnected When Vacuum Applied')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30007, 3, N'Faible niveau de fluide frigorigène dans le réservoir', 'Vérifiez le temps d’ablation résiduel affiché pour vous assurer qu’il y ait suffisamment de fluide frigorigène pour terminer le cas. 

Remplacez le réservoir au besoin.', 1, 32, N'Warning- 00000020 Low Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30008, 3, N'Niveau de fluide frigorigène insuffisant pour effectuer une procédure', 'Remplacer le réservoir de fluide frigorigène.', 1, 64, N'Error 2- 00000040 Insufficient Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (30009, 3, N'La console a détecté que le cathéter a été débranché électriquement pendant le traitement', 'Débranchez et rebranchez l’ICB de la console. Si le problème persiste, débranchez et rebranchez le câble électrique du cathéter de l’ICB puis le cathéter. Appliquer du vide pour continuer. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 128, N'Error 2- 00000080 Catheter is disconnected during treatment')

INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300010, 3, N'Tank pressure is too high', 'Solution', 1, 256, N'Warning- 00000100 Tank pressure is too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300011, 3, N'La pression du réservoir est trop basse.', 'Assurez-vous que la vanne du réservoir de fluide frigorigène est ouverte. Si le problème persiste, remplacez le réservoir. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 512, N'Warning- 00000200 Tank pressure is too low.Please open tank or replace.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300012, 3, N'La pression du réservoir est trop élevée.', 'Assurez-vous que les ventilateurs de la console fonctionnent. Ouvrez la porte du réservoir et éteignez la console. Si les ventilateurs de la console fonctionnent, attendez au moins 10 minutes avant de recommencer. Sinon, ou si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 1024, N'Error 2- 00000400 Tank pressure is too high.Please open tank door.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300013, 3, N'La console a détecté un problème logiciel', 'Débranchez l’ICB de la console et redémarrez la console. Une fois que la console aura redémarré, branchez l’ICB à la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 2048, N'Error 2- 00000800 GUI Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300014, 3, N'La pression d’injection est trop élevée', 'Remplacez le cryocâble et essayez une autre ablation. Si le problème persiste, remplacez le cathéter. 
Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 4096, N'Error 2- 00001000 Injection pressure too high.(PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300015, 3, N'La console a détecté un problème matériel', 'Communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 8192, N'Error 2- 00002000 Injection pressure reading out of range. (PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300016, 3, N'Obstruction du débit détectée', 'Débranchez et rebranchez
le cryocâble. Si le problème persiste, remplacez le cathéter. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 16384, N'Error 2- 00004000 Obstruction detected. Return pressure too high. (PT3)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300017, 3, N'La console a détecté un problème matériel', 'Débranchez l’ICB de la console et redémarrez la console. Une fois que la console aura redémarré, branchez l’ICB à la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 32768, N'Error 2- 00008000 -	Control Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300018, 3, N'Obstruction du débit détectée', 'Essayez une autre ablation. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 65536, N'Error 2- 00010000 Obstruction detected. Insufficient vacuum level. (PT4)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300019, 3, N'La console a détecté un problème matériel', 'Débranchez l’ICB de la console et redémarrez la console. Une fois que la console aura redémarré, branchez l’ICB à la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 131072, N'Error 2- 00020000 -	Patient Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300020, 3, N'La température du refroidisseur secondaire est trop élevée', 'Attendez 5 minutes avant d’essayer l’ablation suivante. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 262144, N'Warning- 00040000 Subcooler temperature too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300021, 3, N'Subcooler temperature out of range', 'Solution', 1, 524288, N'Error 2- 00080000 Subcooler temperature out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300022, 3, N'La console a détecté un problème matériel', 'Attendez 5 minutes avant d’essayer l’ablation suivante. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 1048576, N'Error 2- 00100000 Venting line error detected. (PS1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300023, 3, N'Venting line error out of range', 'Solution', 1, 2097152, N'Error 2- 00200000 Venting line error out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300024, 3, N'La pression de la ligne d’évacuation est trop élevée', 'Assurez-vous que le système d’évacuation de l’hôpital
est sous tension et que le tuyau d’évacuation est solidement fixé. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 4194304, N'Error 2- 00400000 Scavenging line pressure too high(PT5)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300025, 3, N'Catheter Tube Connected', 'Solution', 1, 33554432, N'Catheter Tube Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300026, 3, N'Échec du test automatique de la console.', 'Réinitialisez la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 1, 67108864, N'Error 2- 04000000 CMCU Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300027, 3, N'Scavenging line pressure reading out of range', 'Solution', 1, 8388608, N'Error 2- 00800000 Scavenging line pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300028, 3, N'Vein Isolated', 'Solution', 1, 16777216, N'Vein Isolated')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300029, 3, N'CMCU Ready', 'Solution', 1, 134217728, N'CMCU Ready')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300030, 3, N'System has detected a hardware problem', 'Report the System Notice number to Cryterion Medical technical support.', 2, 1, N'Error 1- 00000001 Hardware Error – CPLD WDT')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300031, 3, N'Échec du test automatique de la console', 'Réinitialisez la console. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 2, 2, N'Error 1- 00000002 Inner balloon pressure too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300032, 3, N'La pression intérieure du ballonnet est trop élevée', 'Essayez une autre ablation. Si le problème persiste, remplacez le cryocâble puis le cathéter. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 2, 4, N'Error 1- 00000004 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300033, 3, N'La pression intérieure du ballonnet est trop basse', 'Essayez une autre ablation. Si le problème persiste, remplacez le cathéter. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 2, 8, N'Error 1- 00000008 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300034, 3, N'Outer balloon breach detected', 'Replace the catheter', 2, 16, N'Error 1- 00000010 Outer balloon breach detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300035, 3, N'La pression extérieure du ballonnet est trop élevée', 'Débranchez et rebranchez le cryocâble de la console et le cathéter. Si le problème persiste, remplacez le cathéter et le cryocâble. Si le problème persiste, communiquez avec le soutien technique de Cryterion Medical et indiquez-leur le code d’erreur.', 2, 32, N'Error 1- 00000020 Outer Balloon Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300036, 3, N'Outer balloon pressure reading out of range', 'Solution', 2, 64, N'Error 1- 00000040 Outer balloon pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300037, 3, N'Balloon Tip Pressure Too High', 'Solution', 2, 128, N'Error 1- 00000080 Balloon Tip Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300038, 3, N'Balloon Tip Pressure Too Low', 'Solution', 2, 256, N'Error 1- 00000100 Balloon Tip Pressure Too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300039, 3, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range', 'Solution', 2, 512, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300040, 3, N'Thawing Temperature Too High', 'Solution', 2, 1024, N'Error 1- 00000400 Thawing Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300041, 3, N'Balloon temperature too Low', 'Solution', 2, 2048, N'Error 1- 00000800 Balloon temperature too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300042, 3, N'La température du ballonnet est trop basse. Le cathéter pourrait être trop profondément inséré dans la veine', 'Repositionnez le cathéter essayez une autre ablation.', 2, 4096, N'Error 1- 0001000 Balloon Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300043, 3, N'La console a détecté du sang dans le cathéter', 'Remplacez le cathéter. N’essayez pas d’effectuer d’autres inflations ou ablations avec ce cathéter.', 2, 16384, N'Error 1- 0004000 Blood detected in the catheter. Please replace the catheter')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300045, 3, N'Catheter Cable Connected', 'Solution', 2, 16777216, N'Catheter Cable Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300046, 3, N'System has detected a hardware problem.', 'Solution', 2, 67108864, N'Error 2- 04000000 Pateint Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message],[SolutionMessage],  [Type], [ErrorCode], [CryterionMessage]) VALUES (300048, 3, N'PMCU Ready', 'Solution', 2, 134217728, N'PMCU Ready')

-- *****************************************************************************************
-- GUI MESSAGES - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300049, 3, N'Erreur de chargement de cellule', 'Solution', 3, 26081, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300050, 3, N'Souhaitez-vous réinitialiser?', 'Solution', 3, 26082, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300051, 3, N'Erreur système', 'Solution', 3, 26083, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300052, 3, N'Erreur d’écriture d’ablation', 'Solution', 3, 26084, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300053, 3, N'Erreur d’écriture d’ECG ablation', 'Solution', 3, 26085, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300054, 3, N'Arrêter l’écriture au fichier JSON', 'Solution', 3, 26086, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300055, 3, N'Erreur de chargement du traitement', 'Solution', 3, 26087, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300056, 3, N'Veuillez sélectionner un répertoire', 'Solution', 3, 26088, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300057, 3, N'Êtes-vous sûr de vouloir fermer le logiciel?', 'Solution', 3, 26089, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300058, 3, N'Êtes-vous sûr de vouloir éteindre votre ordinateur maintenant?', 'Solution', 3, 260810, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300059, 3, N'Êtes-vous sûr de vouloir arrêter la procédure?', 'Solution', 3, 260811, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300060, 3, N'Vous n’avez pas les droits suffisants pour accéder aux paramètres.', 'Solution', 3, 260812, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300061, 3, N'Êtes-vous sûr de vouloir arrêter la procédure sans ajouter de notes sur l’issue?', 'Solution', 3, 260813, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300062, 3, N'Êtes-vous sûr de vouloir quitter la procédure?', 'Solution', 3, 260814, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300063, 3, N'Êtes-vous sûr de vouloir vous déconnecter du système?', 'Solution', 3, 260815, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300064, 3, N'Le nom d’utilisateur :', 'Solution', 3, 260816, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300065, 3, N'existe déjà!', 'Solution', 3, 260817, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300066, 3, N'Le nom d’utilisateur existe', 'Solution', 3, 260818, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300067, 3, N'existe déjà mais l’utilisateur est inactif.  Souhaitez-vous le réactiver?', 'Solution', 3, 260819, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300068, 3, N'Réactiver l’utilisateur?"', 'Solution', 3, 260820, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300069, 3, N'Le nom de médecin :', 'Solution', 3, 260821, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300070, 3, N'Le médecin existe déjà', 'Solution', 3, 260822, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300071, 3, N'Souhaitez-vous réellement supprimer l’utilisateur :', 'Solution', 3, 260823, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300072, 3, N'Supprimer l’utilisateur', 'Solution', 3, 260824, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300073, 3, N'Le mot de passe actuel pour :', 'Solution', 3, 260825, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300074, 3, N'n’est pas valide!', 'Solution', 3, 260826, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300075, 3, N'Mot de passe non valide', 'Solution', 3, 260827, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300076, 3, N'Impossible de récupérer le médecin sélectionné!', 'Solution', 3, 260828, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300077, 3, N'Médecin non trouvé', 'Solution', 3, 260829, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300078, 3, N'Un médecin doit être sélectionné!', 'Solution', 3, 260830, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300079, 3, N'Médecin manquant', 'Solution', 3, 260831, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300080, 3, N'La date de naissance du patient n’est pas valide!', 'Solution', 3, 260832, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300081, 3, N'Date non valide', 'Solution', 3, 260833, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300082, 3, N'Cet identifiant de patient existe déjà dans la base de données!', 'Solution', 3, 260834, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300083, 3, N'Le patient existe déjà', 'Solution', 3, 260835, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300084, 3, N'Une erreur s’est produite lors de l’insertion d’un nouveau patient dans la base de données!', 'Solution', 3, 260836, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300085, 3, N'Erreur d’insertion de patient', 'Solution', 3, 260837, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300086, 3, N'Le médecin n’a pas pu être récupéré!', 'Solution', 3, 260838, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300087, 3, N'Une erreur s’est produite lors de la création de la procédure d’ablation!', 'Solution', 3, 260839, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300088, 3, N'Erreur de création de procédure', 'Solution', 3, 260840, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300089, 3, N'Une erreur s’est produite lors de la génération de la liste de clés USB!"', 'Solution', 3, 260841, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300090, 3, N'Erreur de liste de clés USB', 'Solution', 3, 260842, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300091, 3, N'Le chemin spécifié est non valide ou ne peut pas être trouvé!', 'Solution', 3, 260843, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300092, 3, N'Données techniques non enregistrées!', 'Solution', 3, 260844, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300093, 3, N'Accès refusé.  Vous n’avez pas accès au chemin spécifié!', 'Solution', 3, 260845, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300094, 3, N'Le chemin spécifié est non valide!', 'Solution', 3, 260846, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300095, 3, N'Le chemin spécifié est non valide!  Un caractère non pris en charge a été détecté.', 'Solution', 3, 260847, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300096, 3, N'Le fichier cible ou le répertoire n’existent plus!', 'Solution', 3, 260848, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300097, 3, N'Une erreur s’est produite lors de l’enregistrement des fichiers de données techniques sur la clé USB!', 'Solution', 3, 260849, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300098, 3, N'Une erreur s’est produite lors de l’enregistrement des fichiers de données techniques sur la clé USB!', 'Solution', 3, 260850, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300099, 3, N'Les fichiers de données techniques ont été enregistrés avec succès sur la clé USB!', 'Solution', 3, 260851, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300100, 3, N'Données techniques enregistrées avec succès!', 'Solution', 3, 260852, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300101, 3, N'Une erreur s’est produite lors de la génération de la liste de clés USB!', 'Solution', 3, 260853, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300102, 3, N'Erreur de liste de clés USB', 'Solution', 3, 260854, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300103, 3, N'Une erreur s’est produite lors de l’enregistrement de l’issue de la procédure dans la base de données!', 'Solution', 3, 260855, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300104, 3, N'Erreur d’enregistrement de l’issue', 'Solution', 3, 260856, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300105, 3, N'Une erreur s’est produite lors de l’enregistrement du diagnostic de la procédure dans la base de données!', 'Solution', 3, 260857, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300106, 3, N'Erreur d’enregistrement du diagnostic', 'Solution', 3, 260858, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300107, 3, N'L’information sur l’hôpital n’est pas valide', 'Solution', 3, 260859, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300108, 3, N'Une erreur s’est produite lors de la génération de la liste de clés USB!', 'Solution', 3, 260860, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300109, 3, N'Erreur de liste de clés USB', 'Solution', 3, 260861, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300110, 3, N'Une erreur s’est produite lors de la génération de la liste de dossiers de procédures!', 'Solution', 3, 260862, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300111, 3, N'Erreur de dossiers de procédures', 'Solution', 3, 260863, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300112, 3, N'La procédure a été enregistrée avec succès sur la clé USB!', 'Solution', 3, 260864, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300113, 3, N'Procédure enregistrée avec succès!', 'Solution', 3, 260865, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300114, 3, N'Le chemin spécifié est non valide ou ne peut pas être trouvé!', 'Solution', 3, 260866, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300115, 3, N'Procédure non enregistrée!', 'Solution', 3, 260867, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300116, 3, N'Accès refusé.  Vous n’avez pas accès au chemin spécifié!', 'Solution', 3, 260868, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300117, 3, N'Le chemin spécifié est non valide!', 'Solution', 3, 260869, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300118, 3, N'Le chemin spécifié est non valide!  Un caractère non pris en charge a été détecté.', 'Solution', 3, 260870, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300119, 3, N'Le fichier cible ou le répertoire n’existent plus!', 'Solution', 3, 260871, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300120, 3, N'Une erreur s’est produite lors de l’enregistrement de la procédure sur la clé USB!', 'Solution', 3, 260872, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300121, 3, N'Souhaitez-vous réellement supprimer la liste des messages d’avertissement?', 'Solution', 3, 260873, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300122, 3, N'Supprimer la liste des messages d’erreur', 'Solution', 3, 260874, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300123, 3, N'Il s’est produit une erreur lors de la mise à jour des tableaux de pression embout/ballonnet!', 'Solution', 3, 260875, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300124, 3, N'Erreur de tableau de pression embout/ballonnet', 'Solution', 3, 260876, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300125, 3, N'Il s’est produit une erreur lors du chargement dans les tableaux!"', 'Solution', 3, 260877, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300126, 3, N'Erreur de tableau de température/mouvement du diaphragme"', 'Solution', 3, 260878, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300127, 3, N'Il s’est produit une erreur lors de la tentative d’affichage des notes de traitement.', 'Solution', 3, 260879, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300128, 3, N'Erreur de notes de traitement', 'Solution', 3, 260880, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300129, 3, N'Il s’est produit une erreur pendant l’ablation', 'Solution', 3, 260881, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300130, 3, N'CAN1 Communication perdue ', 'Solution', 3, 260882, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300131, 3, N'CAN2 Communication perdue', 'Solution', 3, 260883, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300132, 3, N'This language is not supported in this version yet.', 'Solution', 3, 260884, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (300133, 3, N'Please restart the system to apply new language settings.', 'Solution', 3, 260885, 'N/A')
-- *****************************************************************************************
-- FRENCH TRANSLATION - END
-- *****************************************************************************************

-- *****************************************************************************************
-- SPANISH TRANSLATION - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60001, 6, N'La consola ha detectado un problema de hardware.', 'Desconecte el ICB de la consola y reinicie la consola. Una vez que la consola haya terminado de reiniciarse, conecte el ICB a la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 1, N'CPLD Watch Dog Timer Error')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60002, 6, N'La consola ha detectado un problema de hardware.', 'Desconecte el ICB de la consola y reinicie la consola. Una vez que la consola haya terminado de reiniciarse, conecte el ICB a la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 2,  N'CMCU Two Multiplex Reading Does Not Match')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60003, 6, N'Se ha detectado un alto flujo de refrigerante', 'Desconecte el criocable, vuelva a conectarlo e intente realizar otra ablación. Si el problema persiste, reemplace el criocable y, a continuación, el catéter. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 4, N'Error 2- 00000004 High refrigerant flow detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60004, 6, N'Se ha detectado una obstrucción en el flujo del refrigerante.', 'Desconecte el criocable, vuelva a conectarlo e intente realizar otra ablación. Si el problema persiste, reemplace el criocable y, a continuación, el catéter. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 8, N'Error 2- 00000008 Refrigerant flow obstruction detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60006, 6, N'El catéter se desconectó en forma mecánica mientras se aplicaba el vacío', 'Asegúrese de que el criocable se encuentre correctamente conectado tanto a la consola como al catéter. Si el problema persiste, cambie el criocable y, a continuación, el catéter. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 16, N'Error 2- 00000010 Catheter Disconnected When Vacuum Applied')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60007, 6, N'Bajo nivel de refrigerante en el tanque', 'Compruebe que se muestre el tiempo restante para completar la ablación, a fin de asegurarse de que haya suficiente refrigerante para terminar el procedimiento. 

Reemplace el tanque, si fuera necesario.', 1, 32, N'Warning- 00000020 Low Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60008, 6, N'Nivel insuficiente de refrigerante en el tanque para realizar un procedimiento', 'Reemplace el tanque de refrigerante.', 1, 64, N'Error 2- 00000040 Insufficient Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (60009, 6, N'La consola detectó que el catéter se conectó en forma eléctrica durante el tratamiento', 'Desconecte y vuelva a conectar el ICB de la consola. Si el problema persiste, desconecte y vuelva a conectar el cable eléctrico del catéter del ICB y, a continuación, el catéter. Aplique vacío para continuar. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 128, N'Error 2- 00000080 Catheter is disconnected during treatment')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600010, 6, N'Tank pressure is too high', 'Solution', 1, 256, N'Warning- 00000100 Tank pressure is too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600011, 6, N'La presión del tanque es baja.', 'Asegúrese de que esté abierta la válvula del tanque de refrigerante. Si el problema persiste, reemplace el tanque. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 512, N'Warning- 00000200 Tank pressure is too low.Please open tank or replace.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600012, 6, N'La presión del tanque es demasiado alta.', 'Asegúrese de que los enfriadores de la consola estén funcionando. Abra la puerta del tanque y apague la consola. Si los enfriadores de la consola están funcionando, espere al menos 10 minutos antes de reiniciarla. De lo contrario, o si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 1024, N'Error 2- 00000400 Tank pressure is too high.Please open tank door.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600013, 6, N'La consola ha detectado un problema de software.', 'Desconecte el ICB de la consola y reinicie la consola. Una vez que la consola haya terminado de reiniciarse, conecte el ICB a la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 2048, N'Error 2- 00000800 GUI Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600014, 6, N'La presión de la inyección es demasiado alta', 'Reemplace el criocable e intente realizar otra ablación. Si el problema persiste, reemplace el catéter. 
Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 4096, N'Error 2- 00001000 Injection pressure too high.(PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600015, 6, N'La consola ha detectado un problema de hardware', 'Comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 8192, N'Error 2- 00002000 Injection pressure reading out of range. (PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600016, 6, N'Se ha detectado una obstrucción en el flujo', 'Desconecte el criocable y vuelva a
conectarlo. Si el problema persiste, reemplace el catéter. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 16384, N'Error 2- 00004000 Obstruction detected. Return pressure too high. (PT3)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600017, 6, N'La consola ha detectado un problema de hardware', 'Desconecte el ICB de la consola y reinicie la consola. Una vez que la consola haya terminado de reiniciarse, conecte el ICB a la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 32768, N'Error 2- 00008000 -	Control Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600018, 6, N'Se ha detectado una obstrucción en el flujo', 'Intente realizar otra ablación. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 65536, N'Error 2- 00010000 Obstruction detected. Insufficient vacuum level. (PT4)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600019, 6, N'La consola ha detectado un problema de hardware', 'Desconecte el ICB de la consola y reinicie la consola. Una vez que la consola haya terminado de reiniciarse, conecte el ICB a la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 131072, N'Error 2- 00020000 -	Patient Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600020, 6, N'La temperatura del subenfriador es demasiado alta', 'Espere cinco (5) minutos antes de intentar la próxima ablación. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 262144, N'Warning- 00040000 Subcooler temperature too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600021, 6, N'Subcooler temperature out of range', 'Solution', 1, 524288, N'Error 2- 00080000 Subcooler temperature out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600022, 6, N'La consola ha detectado un problema de hardware', 'Espere cinco (5) minutos antes de intentar la próxima ablación. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 1048576, N'Error 2- 00100000 Venting line error detected. (PS1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600023, 6, N'Venting line error out of range', 'Solution', 1, 2097152, N'Error 2- 00200000 Venting line error out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600024, 6, N'La presión de la vía de depuración es demasiado alta', 'Asegúrese de que el sistema hospitalario de
depuración esté encendido y de que la manguera de depuración se encuentre correctamente conectada. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 4194304, N'Error 2- 00400000 Scavenging line pressure too high(PT5)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600025, 6, N'Catheter Tube Connected', 'Solution', 1, 33554432, N'Catheter Tube Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600026, 6, N'La consola no ha pasado la prueba automática.', 'Reinicie la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 1, 67108864, N'Error 2- 04000000 CMCU Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600027, 6, N'Scavenging line pressure reading out of range', 'Solution', 1, 8388608, N'Error 2- 00800000 Scavenging line pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600028, 6, N'Vein Isolated', 'Solution', 1, 16777216, N'Vein Isolated')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600029, 6, N'CMCU Ready', 'Solution', 1, 134217728, N'CMCU Ready')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600030, 6, N'System has detected a hardware problem', 'Report the System Notice number to Cryterion Medical technical support.', 2, 1, N'Error 1- 00000001 Hardware Error – CPLD WDT')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600031, 6, N'La consola no ha pasado la prueba automática', 'Reinicie la consola. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 2, 2, N'Error 1- 00000002 Inner balloon pressure too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600032, 6, N'La presión del balón interno es demasiado alta', 'Intente realizar otra ablación. Si el problema persiste, reemplace el criocable y, a continuación, el catéter. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 2, 4, N'Error 1- 00000004 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600033, 6, N'La presión del balón interno es demasiado baja', 'Intente realizar otra ablación. Si el problema persiste, reemplace el catéter. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 2, 8, N'Error 1- 00000008 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600034, 6, N'Outer balloon breach detected', 'Replace the catheter', 2, 16, N'Error 1- 00000010 Outer balloon breach detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600035, 6, N'La presión del balón externo es demasiado alta', 'Desconecte y vuelva a conectar el criocable de la consola y el catéter. Si el problema persiste, reemplace el catéter y el criocable. Si el problema persiste, comuníquese con el servicio de asistencia técnica de Cryterion Medical y proporcione el código del error.', 2, 32, N'Error 1- 00000020 Outer Balloon Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600036, 6, N'Outer balloon pressure reading out of range', 'Solution', 2, 64, N'Error 1- 00000040 Outer balloon pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600037, 6, N'Balloon Tip Pressure Too High', 'Solution', 2, 128, N'Error 1- 00000080 Balloon Tip Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600038, 6, N'Balloon Tip Pressure Too Low', 'Solution', 2, 256, N'Error 1- 00000100 Balloon Tip Pressure Too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600039, 6, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range', 'Solution', 2, 512, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600040, 6, N'Thawing Temperature Too High', 'Solution', 2, 1024, N'Error 1- 00000400 Thawing Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600041, 6, N'Balloon temperature too Low', 'Solution', 2, 2048, N'Error 1- 00000800 Balloon temperature too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600042, 6, N'La temperatura del balón es demasiado baja. Es posible que el catéter se haya hundido demasiado en la vena', 'Acomode el catéter e intente realizar otra ablación.', 2, 4096, N'Error 1- 0001000 Balloon Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600043, 6, N'La consola ha detectado sangre en el catéter', 'Reemplace el catéter. No intente realizar más distensiones o ablaciones con este catéter.', 2, 16384, N'Error 1- 0004000 Blood detected in the catheter. Please replace the catheter')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600045, 6, N'Catheter Cable Connected', 'Solution', 2, 16777216, N'Catheter Cable Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600046, 6, N'System has detected a hardware problem.', 'Solution', 2, 67108864, N'Error 2- 04000000 Pateint Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message],[SolutionMessage],  [Type], [ErrorCode], [CryterionMessage]) VALUES (600048, 6, N'PMCU Ready', 'Solution', 2, 134217728, N'PMCU Ready')

-- *****************************************************************************************
-- GUI MESSAGES - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600049, 6, N'Error de celda de carga', 'Solution', 3, 26081, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600050, 6, N'¿Desea restablecerlo?', 'Solution', 3, 26082, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600051, 6, N'Error de sistema', 'Solution', 3, 26083, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600052, 6, N'Error de escritura de ablación', 'Solution', 3, 26084, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600053, 6, N'Error de escritura de ECG de ablación', 'Solution', 3, 26085, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600054, 6, N'Detener la escritura en el archivo JSON', 'Solution', 3, 26086, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600055, 6, N'Error de carga de tratamiento', 'Solution', 3, 26087, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600056, 6, N'Seleccione un registro.', 'Solution', 3, 26088, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600057, 6, N'¿Está seguro de que desea cerrar el software?', 'Solution', 3, 26089, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600058, 6, N'¿Está seguro de que desea apagar el ordenador ahora?', 'Solution', 3, 260810, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600059, 6, N'¿Está seguro de que desea finalizar el procedimiento?', 'Solution', 3, 260811, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600060, 6, N'No tiene los privilegios suficientes para acceder a la Configuración.', 'Solution', 3, 260812, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600061, 6, N'¿Está seguro de que desea finalizar el procedimiento sin agregar ninguna nota sobre el resultado?', 'Solution', 3, 260813, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600062, 6, N'¿Está seguro de que desea salir del procedimiento?', 'Solution', 3, 260814, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600063, 6, N'¿Está seguro de que desea cerrar la sesión del sistema?', 'Solution', 3, 260815, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600064, 6, N'Nombre de usuario:', 'Solution', 3, 260816, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600065, 6, N'Ya existe.', 'Solution', 3, 260817, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600066, 6, N'Este nombre de usuario ya existe.', 'Solution', 3, 260818, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600067, 6, N'Ya existe, pero el usuario está inactivo.  ¿Desea reactivarlo?', 'Solution', 3, 260819, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600068, 6, N'¿Desea reactivar el usuario?', 'Solution', 3, 260820, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600069, 6, N'Nombre del médico:', 'Solution', 3, 260821, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600070, 6, N'El médico ya existe.', 'Solution', 3, 260822, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600071, 6, N'¿Está seguro de que desea eliminar el usuario?', 'Solution', 3, 260823, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600072, 6, N'Eliminar usuario', 'Solution', 3, 260824, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600073, 6, N'Contraseña actual para:', 'Solution', 3, 260825, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600074, 6, N'No es válida.', 'Solution', 3, 260826, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600075, 6, N'Contraseña no válida', 'Solution', 3, 260827, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600076, 6, N'No se ha podido recuperar el médico seleccionado.', 'Solution', 3, 260828, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600077, 6, N'No se ha encontrado el médico.', 'Solution', 3, 260829, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600078, 6, N'Debe seleccionar un médico.', 'Solution', 3, 260830, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600079, 6, N'Falta el médico.', 'Solution', 3, 260831, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600080, 6, N'La fecha del nacimiento del paciente no es válida.', 'Solution', 3, 260832, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600081, 6, N'Fecha no válida', 'Solution', 3, 260833, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600082, 6, N'El ID del paciente ya existe en la base de datos.', 'Solution', 3, 260834, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600083, 6, N'El paciente ya existe.', 'Solution', 3, 260835, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600084, 6, N'Se ha producido un error al insertar un nuevo paciente en la base de datos.', 'Solution', 3, 260836, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600085, 6, N'Error de inserción de paciente', 'Solution', 3, 260837, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600086, 6, N'No se ha podido recuperar el médico.', 'Solution', 3, 260838, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600087, 6, N'Se ha producido un error al crear el procedimiento de ablación.', 'Solution', 3, 260839, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600088, 6, N'Error de creación de procedimiento', 'Solution', 3, 260840, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600089, 6, N'Se ha producido un error al generar la lista en la unidad USB.', 'Solution', 3, 260841, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600090, 6, N'Error de lista en unidad USB', 'Solution', 3, 260842, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600091, 6, N'La ruta especificada no es válida o no se encuentra.', 'Solution', 3, 260843, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600092, 6, N'No se han guardado los datos de ingeniería.', 'Solution', 3, 260844, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600093, 6, N'Acceso denegado.  No tiene acceso a la ruta especificada.', 'Solution', 3, 260845, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600094, 6, N'La ruta especificada no es válida.', 'Solution', 3, 260846, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600095, 6, N'La ruta especificada no es válida.  Se ha detectado un carácter no compatible.', 'Solution', 3, 260847, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600096, 6, N'El directorio o archivo de destino ya no existe.', 'Solution', 3, 260848, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600097, 6, N'Se ha producido un error al guardar los archivos de datos de ingeniería en la unidad USB.', 'Solution', 3, 260849, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600098, 6, N'Se ha producido un error al guardar los archivos de datos de ingeniería en la unidad USB.', 'Solution', 3, 260850, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600099, 6, N'Los archivos de datos de ingeniería se han guardado correctamente en la unidad USB.', 'Solution', 3, 260851, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600100, 6, N'Los datos de ingeniería se han guardado correctamente.', 'Solution', 3, 260852, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600101, 6, N'Se ha producido un error al generar la lista en la unidad USB.', 'Solution', 3, 260853, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600102, 6, N'Error de lista en unidad USB', 'Solution', 3, 260854, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600103, 6, N'Se ha producido un error al guardar el resultado del procedimiento en la base de datos.', 'Solution', 3, 260855, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600104, 6, N'Error al guardar el resultado', 'Solution', 3, 260856, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600105, 6, N'Se ha producido un error al guardar el diagnóstico del procedimiento en la base de datos.', 'Solution', 3, 260857, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600106, 6, N'Error al guardar el diagnóstico', 'Solution', 3, 260858, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600107, 6, N'La información del hospital no es válida.', 'Solution', 3, 260859, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600108, 6, N'Se ha producido un error al generar la lista en la unidad USB.', 'Solution', 3, 260860, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600109, 6, N'Error de lista en unidad USB', 'Solution', 3, 260861, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600110, 6, N'Se ha producido un error al generar la lista de registros del procedimiento.', 'Solution', 3, 260862, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600111, 6, N'Error de registros del procedimiento', 'Solution', 3, 260863, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600112, 6, N'El procedimiento se ha guardado correctamente en la unidad USB.', 'Solution', 3, 260864, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600113, 6, N'El procedimiento se ha guardado correctamente.', 'Solution', 3, 260865, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600114, 6, N'La ruta especificada no es válida o no se encuentra.', 'Solution', 3, 260866, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600115, 6, N'No se ha guardado el procedimiento.', 'Solution', 3, 260867, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600116, 6, N'Acceso denegado.  No tiene acceso a la ruta especificada.', 'Solution', 3, 260868, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600117, 6, N'La ruta especificada no es válida.', 'Solution', 3, 260869, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600118, 6, N'La ruta especificada no es válida.  Se ha detectado un carácter no compatible.', 'Solution', 3, 260870, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600119, 6, N'El directorio o archivo de destino ya no existe.', 'Solution', 3, 260871, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600120, 6, N'Se ha producido un error al guardar el procedimiento en la unidad USB.', 'Solution', 3, 260872, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600121, 6, N'¿Está seguro de que desea borrar la lista de mensajes de advertencia?', 'Solution', 3, 260873, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600122, 6, N'Borrar lista de mensajes de advertencia', 'Solution', 3, 260874, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600123, 6, N'Se ha producido un error al actualizar los gráficos de presiones de punta o balón.', 'Solution', 3, 260875, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600124, 6, N'Error de gráficos de presiones de punta/balón', 'Solution', 3, 260876, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600125, 6, N'Se ha producido un error al cargar los gráficos.', 'Solution', 3, 260877, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600126, 6, N'Error de gráficos de movimientos de diafragma/temperatura', 'Solution', 3, 260878, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600127, 6, N'Se ha producido un error al intentar mostrar las notas del tratamiento.', 'Solution', 3, 260879, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600128, 6, N'Error de notas del tratamiento', 'Solution', 3, 260880, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600129, 6, N'Se ha producido un error durante la ablación.', 'Solution', 3, 260881, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600130, 6, N'CAN1 Communication', 'Solution', 3, 260882, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600131, 6, N'CAN2 Communication', 'Solution', 3, 260883, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600132, 6, N'This language is not supported in this version yet.', 'Solution', 3, 260884, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (600133, 6, N'Please restart the system to apply new language settings.', 'Solution', 3, 260885, 'N/A')

-- *****************************************************************************************
-- SPANISH TRANSLATION - END
-- *****************************************************************************************

-- *****************************************************************************************
-- ITALIAN TRANSLATION - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70001, 7, N'La console ha rilevato un problema di hardware.', 'Scollegare l’ICB dalla console e riavviare la console. Dopo che la console ha terminato il riavvio, collegare l’ICB alla console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 1, N'CPLD Watch Dog Timer Error')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70002, 7, N'La console ha rilevato un problema di hardware.', 'Scollegare l’ICB dalla console e riavviare la console. Dopo che la console ha terminato il riavvio, collegare l’ICB alla console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 2,  N'CMCU Two Multiplex Reading Does Not Match')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70003, 7, N'Rilevato alto flusso refrigerante', 'Scollegare e ricollegare il cavo criogenico e provare un’altra ablazione. Se il problema persiste, sostituire il cavo criogenico e poi il catetere. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 4, N'Error 2- 00000004 High refrigerant flow detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70004, 7, N'Rilevata ostruzione al flusso di refrigerante', 'Scollegare e ricollegare il cavo criogenico e provare un’altra ablazione. Se il problema persiste, sostituire il cavo criogenico e poi il catetere. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 8, N'Error 2- 00000008 Refrigerant flow obstruction detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70006, 7, N'Il catetere è stato scollegato meccanicamente durante l’applicazione del vuoto', 'Assicurarsi che il cavo criogenico sia collegato correttamente sia alla console che al catetere. Se il problema persiste, sostituire il cavo criogenico e poi il catetere. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 16, N'Error 2- 00000010 Catheter Disconnected When Vacuum Applied')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70007, 7, N'Basso livello di refrigerante nel serbatoio', 'Controllare il tempo di ablazione rimanente visualizzato per garantire che vi sia abbastanza refrigerante per terminare il caso. 

Sostituire il serbatoio se necessario.', 1, 32, N'Warning- 00000020 Low Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70008, 7, N'Livello di refrigerante insufficiente nel serbatoio per eseguire una procedura', 'Sostituire il serbatoio del refrigerante', 1, 64, N'Error 2- 00000040 Insufficient Refrigerant level in tank')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (70009, 7, N'La console ha rilevato che il catetere è stato scollegato elettricamente durante il trattamento', 'Scollegare e ricollegare l’ICB dalla console. Se il problema persiste, scollegare e ricollegare il cavo elettrico del catetere dall’ICB e poi il catetere. Applicare il vuoto per continuare. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 128, N'Error 2- 00000080 Catheter is disconnected during treatment')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700010, 7, N'Tank pressure is too high', 'Solution', 1, 256, N'Warning- 00000100 Tank pressure is too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700011, 7, N'La pressione del serbatoio è troppo bassa.', 'Assicurarsi che la valvola del serbatoio del refrigerante sia aperta. Se il problema persiste, sostituire il serbatoio. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 512, N'Warning- 00000200 Tank pressure is too low.Please open tank or replace.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700012, 7, N'La pressione del serbatoio è troppo alta.', 'Assicurarsi che le ventole della console funzionino. Aprire lo sportello del serbatoio e arrestare la console. Se le ventole della console funzionano, attendere almeno 10 minuti prima di riavviare. Altrimenti, se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 1024, N'Error 2- 00000400 Tank pressure is too high.Please open tank door.(PT1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700013, 7, N'La console ha rilevato un problema di software', 'Scollegare l’ICB dalla console e riavviare la console. Dopo che la console ha terminato il riavvio, collegare l’ICB alla console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 2048, N'Error 2- 00000800 GUI Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700014, 7, N'La pressione di iniezione è troppo alta', 'Sostituire il cavo criogenico e provare un’altra ablazione. Se il problema persiste, sostituire il catetere. 
Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 4096, N'Error 2- 00001000 Injection pressure too high.(PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700015, 7, N'La console ha rilevato un problema di hardware', 'Contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 8192, N'Error 2- 00002000 Injection pressure reading out of range. (PT2)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700016, 7, N'Rilevata ostruzione al flusso', 'Scollegare e ricollegare il
cavo criogenico. Se il problema persiste, sostituire il catetere. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 16384, N'Error 2- 00004000 Obstruction detected. Return pressure too high. (PT3)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700017, 7, N'La console ha rilevato un problema di hardware', 'Scollegare l’ICB dalla console e riavviare la console. Dopo che la console ha terminato il riavvio, collegare l’ICB alla console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 32768, N'Error 2- 00008000 -	Control Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700018, 7, N'Rilevata ostruzione al flusso', 'Provare un’altra ablazione. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 65536, N'Error 2- 00010000 Obstruction detected. Insufficient vacuum level. (PT4)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700019, 7, N'La console ha rilevato un problema di hardware', 'Scollegare l’ICB dalla console e riavviare la console. Dopo che la console ha terminato il riavvio, collegare l’ICB alla console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 131072, N'Error 2- 00020000 -	Patient Microcontroller Watchdog timeout')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700020, 7, N'La temperatura del sottoraffreddatore è troppo alta', 'Attendere 5 minuti prima di provare l’ablazione successiva. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 262144, N'Warning- 00040000 Subcooler temperature too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700021, 7, N'Subcooler temperature out of range', 'Solution', 1, 524288, N'Error 2- 00080000 Subcooler temperature out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700022, 7, N'La console ha rilevato un problema di hardware', 'Attendere 5 minuti prima di provare l’ablazione successiva. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 1048576, N'Error 2- 00100000 Venting line error detected. (PS1)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700023, 7, N'Venting line error out of range', 'Solution', 1, 2097152, N'Error 2- 00200000 Venting line error out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700024, 7, N'La pressione del tubo di lavaggio è troppo alta', 'Verificare che il sistema di lavaggio
dell’ospedale sia acceso e che il tubo flessibile di lavaggio sia collegato saldamente. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 4194304, N'Error 2- 00400000 Scavenging line pressure too high(PT5)')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700025, 7, N'Catheter Tube Connected', 'Solution', 1, 33554432, N'Catheter Tube Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700026, 7, N'La console non ha superato l’autotest.', 'Riavviare la console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 1, 67108864, N'Error 2- 04000000 CMCU Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700027, 7, N'Scavenging line pressure reading out of range', 'Solution', 1, 8388608, N'Error 2- 00800000 Scavenging line pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700028, 7, N'Vein Isolated', 'Solution', 1, 16777216, N'Vein Isolated')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700029, 7, N'CMCU Ready', 'Solution', 1, 134217728, N'CMCU Ready')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700030, 7, N'System has detected a hardware problem', 'Report the System Notice number to Cryterion Medical technical support.', 2, 1, N'Error 1- 00000001 Hardware Error – CPLD WDT')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700031, 7, N'La console non ha superato l’autotest', 'Riavviare la console. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 2, 2, N'Error 1- 00000002 Inner balloon pressure too high')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700032, 7, N'La pressione interna del palloncino è troppo alta', 'Provare un’altra ablazione. Se il problema persiste, sostituire il cavo criogenico e poi il catetere. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 2, 4, N'Error 1- 00000004 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700033, 7, N'La pressione interna del palloncino è troppo bassa', 'Provare un’altra ablazione. Se il problema persiste, sostituire il catetere. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 2, 8, N'Error 1- 00000008 Inner balloon pressure too low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700034, 7, N'Outer balloon breach detected', 'Replace the catheter', 2, 16, N'Error 1- 00000010 Outer balloon breach detected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700035, 7, N'La pressione esterna del palloncino è troppo alta', 'Scollegare e ricollegare il cavo criogenico dalla console e il catetere. Se il problema persiste, sostituire il catetere e il cavo criogenico. Se il problema persiste, contattare l’assistenza tecnica Cryterion Medical e fornire il codice errore.', 2, 32, N'Error 1- 00000020 Outer Balloon Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700036, 7, N'Outer balloon pressure reading out of range', 'Solution', 2, 64, N'Error 1- 00000040 Outer balloon pressure reading out of range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700037, 7, N'Balloon Tip Pressure Too High', 'Solution', 2, 128, N'Error 1- 00000080 Balloon Tip Pressure Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700038, 7, N'Balloon Tip Pressure Too Low', 'Solution', 2, 256, N'Error 1- 00000100 Balloon Tip Pressure Too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700039, 7, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range', 'Solution', 2, 512, N'Error 1- 00000200 Balloon Tip Pressure Reading Out Of Range')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700040, 7, N'Thawing Temperature Too High', 'Solution', 2, 1024, N'Error 1- 00000400 Thawing Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700041, 7, N'Balloon temperature too Low', 'Solution', 2, 2048, N'Error 1- 00000800 Balloon temperature too Low')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700042, 7, N'La temperatura del palloncino è troppo bassa. Il catetere potrebbe essere troppo profondo nella vena', 'Riposizionare il catetere e provare un’altra ablazione.', 2, 4096, N'Error 1- 0001000 Balloon Temperature Too High')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700043, 7, N'La console ha rilevato sangue nel catetere', 'Sostituire il catetere. Non tentare altri gonfiaggi o ablazioni con questo catetere.', 2, 16384, N'Error 1- 0004000 Blood detected in the catheter. Please replace the catheter')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700045, 7, N'Catheter Cable Connected', 'Solution', 2, 16777216, N'Catheter Cable Connected')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700046, 7, N'System has detected a hardware problem.', 'Solution', 2, 67108864, N'Error 2- 04000000 Pateint Self Test Fail')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message],[SolutionMessage],  [Type], [ErrorCode], [CryterionMessage]) VALUES (700048, 7, N'PMCU Ready', 'Solution', 2, 134217728, N'PMCU Ready')

-- *****************************************************************************************
-- GUI MESSAGES - BEGIN
-- *****************************************************************************************
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700049, 7, N'Errore cellule di carico', 'Solution', 3, 26081, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700050, 7, N'Desidera reimpostare?', 'Solution', 3, 26082, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700051, 7, N'Errore sistema', 'Solution', 3, 26083, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700052, 7, N'Errore scrittura ablazione', 'Solution', 3, 26084, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700053, 7, N'Errore scrittura ECG ablazione', 'Solution', 3, 26085, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700054, 7, N'Arrestare scrittura su file JSON', 'Solution', 3, 26086, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700055, 7, N'Errore caricamento trattamento', 'Solution', 3, 26087, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700056, 7, N'Selezionare un registro', 'Solution', 3, 26088, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700057, 7, N'Desidera chiudere il software?', 'Solution', 3, 26089, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700058, 7, N'Desidera spegnere il computer adesso?', 'Solution', 3, 260810, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700059, 7, N'Desidera terminare la procedura?', 'Solution', 3, 260811, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700060, 7, N'Lei non ha privilegi sufficienti per accedere alle Impostazioni.', 'Solution', 3, 260812, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700061, 7, N'Desidera terminare la procedura senza aggiungere note sui risultati?', 'Solution', 3, 260813, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700062, 7, N'Desidera chiudere la procedura?', 'Solution', 3, 260814, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700063, 7, N'Desidera uscire dal sistema?', 'Solution', 3, 260815, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700064, 7, N'Il nome utente:', 'Solution', 3, 260816, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700065, 7, N'esiste già!', 'Solution', 3, 260817, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700066, 7, N'Il Nome utente esiste', 'Solution', 3, 260818, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700067, 7, N'esiste già ma l’utente è inattivo.  Desidera riattivarlo?', 'Solution', 3, 260819, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700068, 7, N'Riattivare utente?”', 'Solution', 3, 260820, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700069, 7, N'Il nome del medico:', 'Solution', 3, 260821, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700070, 7, N'Il medico esiste', 'Solution', 3, 260822, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700071, 7, N'Desidera eliminare l’utente?', 'Solution', 3, 260823, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700072, 7, N'Elimina utente', 'Solution', 3, 260824, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700073, 7, N'La password attuale per:', 'Solution', 3, 260825, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700074, 7, N'non è valida!', 'Solution', 3, 260826, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700075, 7, N'Password non valida', 'Solution', 3, 260827, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700076, 7, N'Impossibile recuperare il Medico selezionato!', 'Solution', 3, 260828, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700077, 7, N'Medico non trovato', 'Solution', 3, 260829, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700078, 7, N'È necessario selezionare un medico!', 'Solution', 3, 260830, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700079, 7, N'Medico mancante', 'Solution', 3, 260831, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700080, 7, N'La data di nascita del paziente non è valida!', 'Solution', 3, 260832, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700081, 7, N'Data non valida', 'Solution', 3, 260833, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700082, 7, N'L’ID di questo paziente esiste già nel database!', 'Solution', 3, 260834, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700083, 7, N'Il paziente esiste già.', 'Solution', 3, 260835, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700084, 7, N'Si è verificato un errore durante l’inserimento di un nuovo paziente nel database!', 'Solution', 3, 260836, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700085, 7, N'Errore di inserimento del paziente', 'Solution', 3, 260837, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700086, 7, N'Impossibile recuperare il Medico!', 'Solution', 3, 260838, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700087, 7, N'Si è verificato un errore durante la creazione della procedura di ablazione!', 'Solution', 3, 260839, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700088, 7, N'Errore di creazione della procedura', 'Solution', 3, 260840, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700089, 7, N'Si è verificato un errore durante la generazione dell’elenco di unità USB!”', 'Solution', 3, 260841, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700090, 7, N'Errore Elenco di unità USB', 'Solution', 3, 260842, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700091, 7, N'Il percorso specificato non è valido o non può essere trovato!', 'Solution', 3, 260843, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700092, 7, N'Dati di ingegneria non salvati!', 'Solution', 3, 260844, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700093, 7, N'Accesso negato. Lei non ha l’accesso al percorso specificato!', 'Solution', 3, 260845, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700094, 7, N'Il percorso specificato non è valido!', 'Solution', 3, 260846, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700095, 7, N'Il percorso specificato non è valido! È stato rilevato un carattere non supportato.', 'Solution', 3, 260847, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700096, 7, N'Il file o la directory target non esistono più!', 'Solution', 3, 260848, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700097, 7, N'Si è verificato un errore durante il salvataggio dei file dei dati di ingegneria sull’unità USB!', 'Solution', 3, 260849, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700098, 7, N'Si è verificato un errore durante il salvataggio dei file dei dati di ingegneria sull’unità USB!', 'Solution', 3, 260850, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700099, 7, N'I file dei dati di ingegneria sono stati salvati sull’unità USB!', 'Solution', 3, 260851, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700100, 7, N'Salvataggio dei dati di ingegneria riuscito!', 'Solution', 3, 260852, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700101, 7, N'Si è verificato un errore durante la generazione dell’elenco di unità USB!', 'Solution', 3, 260853, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700102, 7, N'Errore Elenco di unità USB', 'Solution', 3, 260854, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700103, 7, N'Si è verificato un errore durante il salvataggio dei risultati della procedura nel database!', 'Solution', 3, 260855, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700104, 7, N'Errore salvataggio risultati', 'Solution', 3, 260856, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700105, 7, N'Si è verificato un errore durante il salvataggio della diagnosi della procedura nel database!', 'Solution', 3, 260857, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700106, 7, N'Errore salvataggio diagnosi', 'Solution', 3, 260858, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700107, 7, N'Le informazioni dell’ospedale non sono valide.', 'Solution', 3, 260859, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700108, 7, N'Si è verificato un errore durante la generazione dell’elenco di unità USB!', 'Solution', 3, 260860, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700109, 7, N'Errore Elenco di unità USB', 'Solution', 3, 260861, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700110, 7, N'Si è verificato un errore durante la generazione dell’elenco di record della procedura!', 'Solution', 3, 260862, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700111, 7, N'Errore record della procedura', 'Solution', 3, 260863, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700112, 7, N'La procedura è stata salvata sull’unità USB!', 'Solution', 3, 260864, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700113, 7, N'Salvataggio della procedura riuscito!', 'Solution', 3, 260865, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700114, 7, N'Il percorso specificato non è valido o non può essere trovato!', 'Solution', 3, 260866, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700115, 7, N'Procedura non salvata!', 'Solution', 3, 260867, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700116, 7, N'Accesso negato. Lei non ha l’accesso al percorso specificato!', 'Solution', 3, 260868, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700117, 7, N'Il percorso specificato non è valido!', 'Solution', 3, 260869, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700118, 7, N'Il percorso specificato non è valido! È stato rilevato un carattere non supportato.', 'Solution', 3, 260870, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700119, 7, N'Il file o la directory target non esistono più!', 'Solution', 3, 260871, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700120, 7, N'Si è verificato un errore durante il salvataggio della procedura sull’unità USB!', 'Solution', 3, 260872, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700121, 7, N'Desidera cancellare l’elenco di messaggi di avvertenza?', 'Solution', 3, 260873, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700122, 7, N'Cancella Elenco di messaggi di errore', 'Solution', 3, 260874, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700123, 7, N'Si è verificato un errore durante l’aggiornamento dei grafici di pressione punta/palloncino!', 'Solution', 3, 260875, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700124, 7, N'Errore grafico pressione punta/palloncino', 'Solution', 3, 260876, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700125, 7, N'Si è verificato un errore durante il Caricamento sui grafici!”', 'Solution', 3, 260877, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700126, 7, N'Errore grafico spostamento temperatura/diaframma”', 'Solution', 3, 260878, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700127, 7, N'Si è verificato un errore durante il tentativo di visualizzazione delle note sul trattamento.', 'Solution', 3, 260879, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700128, 7, N'Errore note trattamento', 'Solution', 3, 260880, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700129, 7, N'Si è verificato un errore durante l’ablazione.', 'Solution', 3, 260881, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700130, 7, N'CAN1 Communication', 'Solution', 3, 260882, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700131, 7, N'CAN2 Communication', 'Solution', 3, 260883, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700132, 7, N'This language is not supported in this version yet.', 'Solution', 3, 260884, 'N/A')
INSERT [dbo].[ErrorMessages] ([Id], [LanguageId], [Message], [SolutionMessage], [Type], [ErrorCode], [CryterionMessage]) VALUES (700133, 7, N'Please restart the system to apply new language settings.', 'Solution', 3, 260885, 'N/A')
-- *****************************************************************************************
-- ITALIAN TRANSLATION - END
-- *****************************************************************************************




SET IDENTITY_INSERT [dbo].[ErrorMessages] OFF


SET IDENTITY_INSERT [dbo].[ErrorTypes] ON 

INSERT [dbo].[ErrorTypes] ([Id], [Type], [Description]) VALUES (1, 1, N'CMCU')
INSERT [dbo].[ErrorTypes] ([Id], [Type], [Description]) VALUES (2, 2, N'PMCU')
INSERT [dbo].[ErrorTypes] ([Id], [Type], [Description]) VALUES (3, 3, N'GUI')
SET IDENTITY_INSERT [dbo].[ErrorTypes] OFF

ALTER TABLE [dbo].[ErrorMessages]  WITH CHECK ADD  CONSTRAINT [FK_ErrorErrorMessage] FOREIGN KEY([ErrorCode])
REFERENCES [dbo].[Errors] ([Code])
GO
ALTER TABLE [dbo].[ErrorMessages] CHECK CONSTRAINT [FK_ErrorErrorMessage]
GO