-- CatheterTypes
DELETE FROM CatheterTypes
DBCC CHECKIDENT (CatheterTypes, RESEED, 0)
INSERT INTO CatheterTypes VALUES ('0','-1','-1')

-- SYSTEM STATE
DELETE FROM SystemStates
DBCC CHECKIDENT (SystemStates, RESEED, 0)
INSERT INTO SystemStates VALUES ('1','IDLE STATE')
INSERT INTO SystemStates VALUES ('2','READY STATE')
INSERT INTO SystemStates VALUES ('3','INFLATION STATE')
INSERT INTO SystemStates VALUES ('4','TRANSITION STATE')
INSERT INTO SystemStates VALUES ('5','ABLATION STATE')
INSERT INTO SystemStates VALUES ('6','THAWING STATE')
INSERT INTO SystemStates VALUES ('7','EXCEPTION')

-- CMC Register
-- StateID
-- PT1TankPressureLow, PT1PressureThresholdHighLimit, PT1TankPressureTooHigh,
-- PT1TankPressureLowRangeLimit,PT1TankPressureHighRangeLimit,
-- PT2PressureThresholdHighLimit, PT2PressureLowRangeLimit,PT2PressureHighRangeLimit,
-- PT3PressureThresholdHighLimit,PT3PressureLowRangeLimit,PT3PressureHighRangeLimit,
-- PT4PressureThresholdHighLimit, PT4PressureLowRangeLimit,PT4PressureHighRangeLimit,
-- TS1TemperatureTresholdHighLimit, TS1TemperatureLowRangeLimit,TS1TemperatureHighRangeLimit,
-- FM1FlowMeterThresholdLowLimit,FM1FlowMeterThresholdHighLimit,FM1FlowMeterLowRangeLimit,FM1FlowMeterHighRangeLimit
-- PS1PressureThresholdHighLimit,PS1PressureLowRangeLimit,PS1PressureHighRangeLimit
-- PS2PressureThresholdHighLimit,PS2PressureLowRangeLimit,PS2PressureHighRangeLimit
-- LC1LoadCellThresholdWarning,LC1LoadCellThresholdFail,LC1LoadCellLowRangeLimit,LC1LoadCellHighRangeLimit,
-- PGain, IGain, DGain, 
-- Offset, CatheterTypeID,TargetInjectionFlow
DELETE FROM CMCRegisterValues

-- CMC Register - IDLE
INSERT INTO CMCRegisterValues VALUES ('1',
									  '600','800','1000','50','1200',
									  '20','0','800',
									  '-1','1','29',
									  '5','1','20',
									  '-1','-60','40',
									  '-1','100','0','10000',
									  '100','-1','-1',
									  '7','1','20',
									  '-1','-1','-1','-1',
									  '-1','-1','-1','-1','1','-1')

-- CMC Register - READY
INSERT INTO CMCRegisterValues VALUES ('2',
								      '600','800','1000','50','1200',
								      '100','0','800',
								      '22','1','29',
								      '5','1','20',
								      '-30','-60','40',
								      '-1','-1','0','10000',
								      '100','-1','-1',
								      '7','1','20',
								      '-1','-1','-1','-1',
								      '-1','-1','-1','-1','1','-1')
								   
-- CMC Register - INFLATION
INSERT INTO CMCRegisterValues VALUES ('3',
								      '600','800','1000','50','1200',
								      '100','0','800',
								      '22','1','29',
								      '5','1','20',
								      '-30','-60','40',
								      '-1','-1','0','10000',
								      '100','-1','-1',
								      '7','1','20',
								      '-1','-1','-1','-1',
								      '-1','-1','-1','-1','1','-1')
								   
-- CMC Register - TRANSITION
INSERT INTO CMCRegisterValues VALUES ('4',
								      '600','800','1000','50','1200',
								      '650','0','800',
								      '22','1','29',
								      '5','1','20',
								      '-30','-60','40',
								      '-1','7000','0','10000',
								      '100','-1','-1',
								      '7','1','20',
								      '-1','-1','-1','-1',
								      '-1','-1','-1','-1','1','-1')
								   
-- CMC Register - ABLATION
INSERT INTO CMCRegisterValues VALUES ('5',
								      '600','800','1000','50','1200',
								      '650','0','800',
								      '6','1','29',
								      '5','1','20',
								      '-30','-60','40',
								      '4000','8000','0','10000',
								      '100','-1','-1',
								      '7','1','20',
								      '-1','-1','-1','-1',
								      '-1','-1','-1','-1','1','-1')

-- CMC Register - THAWING
INSERT INTO CMCRegisterValues VALUES ('6',
								      '600','800','1000','50','1200',
								      '20','0','800',
								      '22','1','29',
								      '5','1','20',
								      '-30','-60','40',
								      '-1','-1','0','10000',
								      '100','-1','-1',
								      '7','1','20',
								      '-1','-1','-1','-1',
								      '-1','-1','-1','-1','1','-1')

-- PMC Register Values
-- StateID
-- CP1PressureThresholdHighLimit, CP1PressureLowRangeLimit, CP1PressureHighRangeLimit,
-- CP2PressureThresholdHighLimit, CP2PressureLowRangeLimit, CP2PressureHighRangeLimit,
-- TC1ThawingTemperature
-- Pgain, Igain, Dgain, Offset
-- CatheterTypeID
DELETE FROM PMCRegisterValues

-- PMC Register - IDLE
INSERT INTO PMCRegisterValues VALUES ('1',
									  '-1','-1','-1',
								      '-1','-1','-1',
								      '-1',
								      '-1','-1','-1','-1',
								      '1')

-- PMC Register - READY
INSERT INTO PMCRegisterValues VALUES ('2',
									  '-1','-1','-1',
								      '-1','-1','-1',
								      '-1',
								      '-1','-1','-1','-1',
								      '1')

-- PMC Register - INFLATION
INSERT INTO PMCRegisterValues VALUES ('3',
									  '-1','-1','-1',
								      '-1','-1','-1',
								      '-1',
								      '-1','-1','-1','-1',
								      '1')

-- PMC Register - ABLATION
INSERT INTO PMCRegisterValues VALUES ('5',
									  '-1','-1','-1',
								      '-1','-1','-1',
								      '-1',
								      '-1','-1','-1','-1',
								      '1')

-- PMC Register - THAWING
INSERT INTO PMCRegisterValues VALUES ('6',
									  '-1','-1','-1',
								      '-1','-1','-1',
								      '-1',
								      '-1','-1','-1','-1',
								      '1')

