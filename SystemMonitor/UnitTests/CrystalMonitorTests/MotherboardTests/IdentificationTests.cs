using System.Collections.Generic;
using CrystalMonitor.Hardware.Motherboard;
using Xunit;

namespace CrystalMonitorTests.HardwareTests.MotherboardTests;

public class IdentificationTests {
  // ---------------------------------------------------------------------
  // GetManufacturer — every distinct manufacturer-matching case in the
  // production switch, one row per case label (including duplicate
  // alias labels that fall through to the same return), so each line of
  // the switch gets executed at least once.
  // ---------------------------------------------------------------------
  public static IEnumerable<object[]> ManufacturerCases() {
    return new List<object[]> {
        new object[] { "abit.com.tw", Manufacturer.Acer },
        new object[] { "Acer", Manufacturer.Acer },
        new object[] { "AMD", Manufacturer.AMD },
        new object[] { "Alienware", Manufacturer.Alienware },
        new object[] { "AOpen", Manufacturer.AOpen },
        new object[] { "Apple", Manufacturer.Apple },
        new object[] { "ASRock", Manufacturer.ASRock },
        new object[] { "ASUSTeK", Manufacturer.ASUS },
        new object[] { "ASUS ", Manufacturer.ASUS },
        new object[] { "Biostar", Manufacturer.Biostar },
        new object[] { "Clevo", Manufacturer.Clevo },
        new object[] { "Dell", Manufacturer.Dell },
        new object[] { "DFI", Manufacturer.DFI },
        new object[] { "DFI Inc", Manufacturer.DFI },
        new object[] { "ECS", Manufacturer.ECS },
        new object[] { "ELITEGROUP", Manufacturer.ECS },
        new object[] { "EPoX COMPUTER CO., LTD", Manufacturer.EPoX },
        new object[] { "EVGA", Manufacturer.EVGA },
        new object[] { "FIC", Manufacturer.FIC },
        new object[] { "First International Computer", Manufacturer.FIC },
        new object[] { "Foxconn", Manufacturer.Foxconn },
        new object[] { "Framework", Manufacturer.Framework },
        new object[] { "Fujitsu", Manufacturer.Fujitsu },
        new object[] { "Gigabyte", Manufacturer.Gigabyte },
        new object[] { "Hewlett-Packard", Manufacturer.HP },
        new object[] { "HP", Manufacturer.HP },
        new object[] { "IBM", Manufacturer.IBM },
        new object[] { "Intel", Manufacturer.Intel },
        new object[] { "Intel Corp", Manufacturer.Intel },
        new object[] { "Jetway", Manufacturer.Jetway },
        new object[] { "Lenovo", Manufacturer.Lenovo },
        new object[] { "LattePanda", Manufacturer.LattePanda },
        new object[] { "Medion", Manufacturer.Medion },
        new object[] { "Microsoft", Manufacturer.Microsoft },
        new object[] { "Micro-Star International", Manufacturer.MSI },
        new object[] { "MSI", Manufacturer.MSI },
        new object[] { "NEC ", Manufacturer.NEC },
        new object[] { "NEC", Manufacturer.NEC },
        new object[] { "Pegatron", Manufacturer.Pegatron },
        new object[] { "Samsung", Manufacturer.Samsung },
        new object[] { "Sapphire", Manufacturer.Sapphire },
        new object[] { "Shuttle", Manufacturer.Shuttle },
        new object[] { "Sony", Manufacturer.Sony },
        new object[] { "Supermicro", Manufacturer.Supermicro },
        new object[] { "Toshiba", Manufacturer.Toshiba },
        new object[] { "XFX", Manufacturer.XFX },
        new object[] { "Zotac", Manufacturer.Zotac },
        new object[] { "To be filled by O.E.M.", Manufacturer.Unknown },    };
  }

  [Theory]
  [MemberData(nameof(ManufacturerCases))]
  public void GetManufacturer_MapsKnownNames_ToExpectedManufacturer(string name, Manufacturer expected) {
    Assert.Equal(expected, Identification.GetManufacturer(name));
  }

  // ---------------------------------------------------------------------
  // GetModel — every distinct model-matching case in the production
  // switch, one row per case label.
  // ---------------------------------------------------------------------
  public static IEnumerable<object[]> ModelCases() {
    return new List<object[]> {
        new object[] { "TUF GAMING B850M-PLUS II", Model.TUF_GAMING_B850M_PLUS_II },
        new object[] { "MPG Z390 GAMING EDGE AC (MS-7B17)", Model.Z390_GAMING_EDGE_AC },
        new object[] { "X870 AORUS ELITE WIFI7", Model.X870_AORUS_ELITE_WIFI7 },
        new object[] { "X870 AORUS ELITE WIFI7 ICE", Model.X870_AORUS_ELITE_WIFI7_ICE },
        new object[] { "880GMH/USB3", Model._880GMH_USB3 },
        new object[] { "B85M-DGS", Model.B85M_DGS },
        new object[] { "ASRock AOD790GX/128M", Model.AOD790GX_128M },
        new object[] { "AB350 Pro4", Model.AB350_Pro4 },
        new object[] { "AB350M Pro4", Model.AB350M_Pro4 },
        new object[] { "AB350M", Model.AB350M },
        new object[] { "B450 Steel Legend", Model.B450_Steel_Legend },
        new object[] { "B450M Steel Legend", Model.B450M_Steel_Legend },
        new object[] { "B450 Pro4", Model.B450_Pro4 },
        new object[] { "B450M Pro4", Model.B450M_Pro4 },
        new object[] { "B450M Pro4 R2.0", Model.B450M_Pro4_R2_0 },
        new object[] { "B550M Pro4", Model.B550M_Pro4 },
        new object[] { "Fatal1ty AB350 Gaming K4", Model.Fatal1ty_AB350_Gaming_K4 },
        new object[] { "AB350M-HDV", Model.AB350M_HDV },
        new object[] { "X399 Phantom Gaming 6", Model.X399_Phantom_Gaming_6 },
        new object[] { "A320M-HDV", Model.A320M_HDV },
        new object[] { "P55 Deluxe", Model.P55_Deluxe },
        new object[] { "Crosshair III Formula", Model.CROSSHAIR_III_FORMULA },
        new object[] { "ROG CROSSHAIR VIII HERO", Model.ROG_CROSSHAIR_VIII_HERO },
        new object[] { "ROG CROSSHAIR VIII HERO (WI-FI)", Model.ROG_CROSSHAIR_VIII_HERO_WIFI },
        new object[] { "ROG CROSSHAIR VIII DARK HERO", Model.ROG_CROSSHAIR_VIII_DARK_HERO },
        new object[] { "ROG CROSSHAIR VIII FORMULA", Model.ROG_CROSSHAIR_VIII_FORMULA },
        new object[] { "ROG CROSSHAIR VIII IMPACT", Model.ROG_CROSSHAIR_VIII_IMPACT },
        new object[] { "PRIME B650-PLUS", Model.PRIME_B650_PLUS },
        new object[] { "ROG CROSSHAIR X670E EXTREME", Model.ROG_CROSSHAIR_X670E_EXTREME },
        new object[] { "ROG CROSSHAIR X670E HERO", Model.ROG_CROSSHAIR_X670E_HERO },
        new object[] { "ROG CROSSHAIR X670E GENE", Model.ROG_CROSSHAIR_X670E_GENE },
        new object[] { "PROART X670E-CREATOR WIFI", Model.PROART_X670E_CREATOR_WIFI },
        new object[] { "M2N-SLI DELUXE", Model.M2N_SLI_Deluxe },
        new object[] { "M4A79XTD EVO", Model.M4A79XTD_EVO },
        new object[] { "P5W DH Deluxe", Model.P5W_DH_Deluxe },
        new object[] { "P6T", Model.P6T },
        new object[] { "P6X58D-E", Model.P6X58D_E },
        new object[] { "P8P67", Model.P8P67 },
        new object[] { "P8P67 REV 3.1", Model.P8P67 },
        new object[] { "P8P67 EVO", Model.P8P67_EVO },
        new object[] { "P8P67 PRO", Model.P8P67_PRO },
        new object[] { "P8P67-M PRO", Model.P8P67_M_PRO },
        new object[] { "P8Z77-V", Model.P8Z77_V },
        new object[] { "P9X79", Model.P9X79 },
        new object[] { "Rampage Extreme", Model.RAMPAGE_EXTREME },
        new object[] { "Rampage II GENE", Model.RAMPAGE_II_GENE },
        new object[] { "LP BI P45-T2RS Elite", Model.LP_BI_P45_T2RS_Elite },
        new object[] { "ROG STRIX B550-F GAMING (WI-FI)", Model.ROG_STRIX_B550_F_GAMING_WIFI },
        new object[] { "ROG STRIX X470-I GAMING", Model.ROG_STRIX_X470_I },
        new object[] { "ROG STRIX B550-E GAMING", Model.ROG_STRIX_B550_E_GAMING },
        new object[] { "ROG STRIX B550-I GAMING", Model.ROG_STRIX_B550_I_GAMING },
        new object[] { "ROG STRIX B760-I GAMING WIFI", Model.ROG_STRIX_B760_I_GAMING_WIFI },
        new object[] { "ROG STRIX X570-E GAMING", Model.ROG_STRIX_X570_E_GAMING },
        new object[] { "ROG STRIX X570-E GAMING WIFI II", Model.ROG_STRIX_X570_E_GAMING_WIFI_II },
        new object[] { "ROG STRIX X570-I GAMING", Model.ROG_STRIX_X570_I_GAMING },
        new object[] { "ROG STRIX X570-F GAMING", Model.ROG_STRIX_X570_F_GAMING },
        new object[] { "LP DK P55-T3eH9", Model.LP_DK_P55_T3EH9 },
        new object[] { "A890GXM-A", Model.A890GXM_A },
        new object[] { "X58 SLI Classified", Model.X58_SLI_Classified },
        new object[] { "132-BL-E758", Model.X58_3X_SLI },
        new object[] { "965P-S3", Model._965P_S3 },
        new object[] { "EP45-DS3R", Model.EP45_DS3R },
        new object[] { "EP45-UD3R", Model.EP45_UD3R },
        new object[] { "EX58-EXTREME", Model.EX58_EXTREME },
        new object[] { "EX58-UD3R", Model.EX58_UD3R },
        new object[] { "G41M-Combo", Model.G41M_COMBO },
        new object[] { "G41MT-S2", Model.G41MT_S2 },
        new object[] { "G41MT-S2P", Model.G41MT_S2P },
        new object[] { "970A-DS3P", Model._970A_DS3P },
        new object[] { "970A-DS3P FX", Model._970A_DS3P },
        new object[] { "GA-970A-UD3", Model._970A_UD3 },
        new object[] { "GA-MA770T-UD3", Model.MA770T_UD3 },
        new object[] { "GA-MA770T-UD3P", Model.MA770T_UD3P },
        new object[] { "GA-MA785GM-US2H", Model.MA785GM_US2H },
        new object[] { "GA-MA785GMT-UD2H", Model.MA785GMT_UD2H },
        new object[] { "GA-MA78LM-S2H", Model.MA78LM_S2H },
        new object[] { "GA-MA790X-UD3P", Model.MA790X_UD3P },
        new object[] { "GA-MA790X-DS4", Model.MA790X_DS4 },
        new object[] { "H55-USB3", Model.H55_USB3 },
        new object[] { "H55N-USB3", Model.H55N_USB3 },
        new object[] { "H61M-DGS", Model.H61M_DGS },
        new object[] { "H61M-DS2 REV 1.2", Model.H61M_DS2_REV_1_2 },
        new object[] { "H61M-USB3-B3 REV 2.0", Model.H61M_USB3_B3_REV_2_0 },
        new object[] { "H67A-UD3H-B3", Model.H67A_UD3H_B3 },
        new object[] { "H67A-USB3-B3", Model.H67A_USB3_B3 },
        new object[] { "H97-D3H-CF", Model.H97_D3H },
        new object[] { "H81M-HD3", Model.H81M_HD3 },
        new object[] { "B75M-D3H", Model.B75M_D3H },
        new object[] { "P35-DS3", Model.P35_DS3 },
        new object[] { "P35-DS3L", Model.P35_DS3L },
        new object[] { "P55-UD4", Model.P55_UD4 },
        new object[] { "P55A-UD3", Model.P55A_UD3 },
        new object[] { "P55M-UD4", Model.P55M_UD4 },
        new object[] { "P67A-UD3-B3", Model.P67A_UD3_B3 },
        new object[] { "P67A-UD3R-B3", Model.P67A_UD3R_B3 },
        new object[] { "P67A-UD4-B3", Model.P67A_UD4_B3 },
        new object[] { "P8Z68-V PRO", Model.P8Z68_V_PRO },
        new object[] { "X38-DS5", Model.X38_DS5 },
        new object[] { "X58A-UD3R", Model.X58A_UD3R },
        new object[] { "Z270 PC MATE", Model.Z270_PC_MATE },
        new object[] { "Z270 PC MATE (MS-7A72)", Model.Z270_PC_MATE },
        new object[] { "Z77 MPower", Model.Z77_MS7751 },
        new object[] { "Z77 MPower (MS-7751)", Model.Z77_MS7751 },
        new object[] { "Z77A-GD65", Model.Z77_MS7751 },
        new object[] { "Z77A-GD65 (MS-7751)", Model.Z77_MS7751 },
        new object[] { "Z77A-GD65 GAMING", Model.Z77_MS7751 },
        new object[] { "Z77A-GD65 GAMING (MS-7751)", Model.Z77_MS7751 },
        new object[] { "Z77A-GD55", Model.Z77_MS7751 },
        new object[] { "Z77A-GD55 (MS-7751)", Model.Z77_MS7751 },
        new object[] { "Z77A-GD80", Model.Z77_MS7751 },
        new object[] { "Z77A-GD80 (MS-7757)", Model.Z77_MS7751 },
        new object[] { "Z68A-GD80", Model.Z68_MS7672 },
        new object[] { "Z68A-GD80 (MS-7672)", Model.Z68_MS7672 },
        new object[] { "P67A-GD80", Model.Z68_MS7672 },
        new object[] { "P67A-GD80 (MS-7672)", Model.Z68_MS7672 },
        new object[] { "X79-UD3", Model.X79_UD3 },
        new object[] { "Z68A-D3H-B3", Model.Z68A_D3H_B3 },
        new object[] { "Z68AP-D3", Model.Z68AP_D3 },
        new object[] { "Z68X-UD3H-B3", Model.Z68X_UD3H_B3 },
        new object[] { "Z68X-UD7-B3", Model.Z68X_UD7_B3 },
        new object[] { "Z68XP-UD3R", Model.Z68XP_UD3R },
        new object[] { "Z170N-WIFI-CF", Model.Z170N_WIFI },
        new object[] { "Z390 M GAMING-CF", Model.Z390_M_GAMING },
        new object[] { "Z390 AORUS ULTRA", Model.Z390_AORUS_ULTRA },
        new object[] { "Z390 AORUS PRO-CF", Model.Z390_AORUS_PRO },
        new object[] { "Z390 UD", Model.Z390_UD },
        new object[] { "Z690 AORUS PRO", Model.Z690_AORUS_PRO },
        new object[] { "Z690 AORUS ULTRA", Model.Z690_AORUS_ULTRA },
        new object[] { "Z690 AORUS MASTER", Model.Z690_AORUS_MASTER },
        new object[] { "Z690 GAMING X DDR4", Model.Z690_GAMING_X_DDR4 },
        new object[] { "Z790 AORUS PRO X", Model.Z790_AORUS_PRO_X },
        new object[] { "Z790 AORUS PRO X WIFI7", Model.Z790_AORUS_PRO_X },
        new object[] { "Z790 UD", Model.Z790_UD },
        new object[] { "Z790 UD AC", Model.Z790_UD_AC },
        new object[] { "Z790 GAMING X", Model.Z790_GAMING_X },
        new object[] { "Z790 GAMING X AX", Model.Z790_GAMING_X_AX },
        new object[] { "FH67", Model.FH67 },
        new object[] { "AX370-Gaming K7", Model.AX370_Gaming_K7 },
        new object[] { "PRIME X370-PRO", Model.PRIME_X370_PRO },
        new object[] { "PRIME X470-PRO", Model.PRIME_X470_PRO },
        new object[] { "PRIME X570-PRO", Model.PRIME_X570_PRO },
        new object[] { "ProArt X570-CREATOR WIFI", Model.PROART_X570_CREATOR_WIFI },
        new object[] { "Pro WS X570-ACE", Model.PRO_WS_X570_ACE },
        new object[] { "ROG MAXIMUS X APEX", Model.ROG_MAXIMUS_X_APEX },
        new object[] { "AB350-Gaming 3-CF", Model.AB350_Gaming_3 },
        new object[] { "X399 AORUS Gaming 7", Model.X399_AORUS_Gaming_7 },
        new object[] { "ROG ZENITH EXTREME", Model.ROG_ZENITH_EXTREME },
        new object[] { "ROG ZENITH II EXTREME", Model.ROG_ZENITH_II_EXTREME },
        new object[] { "Z170-A", Model.Z170_A },
        new object[] { "Z170 PRO GAMING", Model.Z170_PRO_GAMING },
        new object[] { "B150M-C", Model.B150M_C },
        new object[] { "B150M-C D3", Model.B150M_C_D3 },
        new object[] { "Z77 Pro4-M", Model.Z77Pro4M },
        new object[] { "X570 Pro4", Model.X570_Pro4 },
        new object[] { "X570 Taichi", Model.X570_Taichi },
        new object[] { "X570 Phantom Gaming-ITX/TB3", Model.X570_Phantom_Gaming_ITX },
        new object[] { "X570 Phantom Gaming 4", Model.X570_Phantom_Gaming_4 },
        new object[] { "AX370-Gaming 5", Model.AX370_Gaming_5 },
        new object[] { "TUF X470-PLUS GAMING", Model.TUF_X470_PLUS_GAMING },
        new object[] { "TUF GAMING X870-PLUS WIFI", Model.TUF_GAMING_X870_PLUS_WIFI },
        new object[] { "B360M PRO-VDH (MS-7B24)", Model.B360M_PRO_VDH },
        new object[] { "A320M-S2H-CF", Model.A320M_S2H_CF },
        new object[] { "B360M H", Model.B360M_H },
        new object[] { "B550-A PRO (MS-7C56)", Model.B550A_PRO },
        new object[] { "PRO B550-VC (MS-7C56)", Model.B550A_PRO },
        new object[] { "B450-A PRO (MS-7B86)", Model.B450A_PRO },
        new object[] { "B350 GAMING PLUS (MS-7A34)", Model.B350_Gaming_Plus },
        new object[] { "B450 AORUS PRO", Model.B450_AORUS_PRO },
        new object[] { "B450 AORUS PRO WIFI", Model.B450_AORUS_PRO },
        new object[] { "B450 GAMING X", Model.B450_GAMING_X },
        new object[] { "B450 AORUS ELITE", Model.B450_AORUS_ELITE },
        new object[] { "B450 AORUS ELITE V2", Model.B450_AORUS_ELITE },
        new object[] { "B450M AORUS ELITE", Model.B450M_AORUS_ELITE },
        new object[] { "B450M AORUS ELITE-CF", Model.B450M_AORUS_ELITE },
        new object[] { "B450M GAMING", Model.B450M_GAMING },
        new object[] { "B450M GAMING-CF", Model.B450M_GAMING },
        new object[] { "B450M AORUS M", Model.B450_AORUS_M },
        new object[] { "B450M AORUS M-CF", Model.B450_AORUS_M },
        new object[] { "B450M DS3H", Model.B450M_DS3H },
        new object[] { "B450M DS3H WIFI", Model.B450M_DS3H },
        new object[] { "B450M DS3H-CF", Model.B450M_DS3H },
        new object[] { "B450M DS3H WIFI-CF", Model.B450M_DS3H },
        new object[] { "B450M DS3H V2", Model.B450M_DS3H },
        new object[] { "B450M DS3H V2-CF", Model.B450M_DS3H },
        new object[] { "B450M S2H", Model.B450M_S2H },
        new object[] { "B450M S2H V2", Model.B450M_S2H },
        new object[] { "B450M S2H-CF", Model.B450M_S2H },
        new object[] { "B450M S2H V2-CF", Model.B450M_S2H },
        new object[] { "B450M H", Model.B450M_H },
        new object[] { "B450M H-CF", Model.B450M_H },
        new object[] { "B450M K", Model.B450M_K },
        new object[] { "B450M K-CF", Model.B450M_K },
        new object[] { "B450M I AORUS PRO WIFI", Model.B450_I_AORUS_PRO_WIFI },
        new object[] { "B450M I AORUS PRO WIFI-CF", Model.B450_I_AORUS_PRO_WIFI },
        new object[] { "X470 AORUS GAMING 7 WIFI-CF", Model.X470_AORUS_GAMING_7_WIFI },
        new object[] { "X570 AORUS MASTER", Model.X570_AORUS_MASTER },
        new object[] { "X570 AORUS PRO", Model.X570_AORUS_PRO },
        new object[] { "X570 AORUS ULTRA", Model.X570_AORUS_ULTRA },
        new object[] { "X570 GAMING X", Model.X570_GAMING_X },
        new object[] { "TUF GAMING X570-PLUS (WI-FI)", Model.TUF_GAMING_X570_PLUS_WIFI },
        new object[] { "TUF GAMING B550M-PLUS (WI-FI)", Model.TUF_GAMING_B550M_PLUS_WIFI },
        new object[] { "TUF GAMING B760M-PLUS WIFI D4", Model.TUF_GAMING_B760M_PLUS_WIFI_D4 },
        new object[] { "B360 AORUS GAMING 3 WIFI-CF", Model.B360_AORUS_GAMING_3_WIFI_CF },
        new object[] { "B550I AORUS PRO AX", Model.B550I_AORUS_PRO_AX },
        new object[] { "B550M AORUS PRO", Model.B550M_AORUS_PRO },
        new object[] { "B550M AORUS PRO-P", Model.B550M_AORUS_PRO },
        new object[] { "B550M AORUS PRO AX", Model.B550M_AORUS_PRO_AX },
        new object[] { "B550M AORUS ELITE", Model.B550M_AORUS_ELITE },
        new object[] { "B550M GAMING", Model.B550M_GAMING },
        new object[] { "B550M DS3H", Model.B550M_DS3H },
        new object[] { "B550M DS3H AC", Model.B550M_DS3H_AC },
        new object[] { "B550M S2H", Model.B550M_S2H },
        new object[] { "B550M H", Model.B550M_H },
        new object[] { "B550 AORUS MASTER", Model.B550_AORUS_MASTER },
        new object[] { "B550 AORUS PRO", Model.B550_AORUS_PRO },
        new object[] { "B550 AORUS PRO V2", Model.B550_AORUS_PRO },
        new object[] { "B550 AORUS PRO AC", Model.B550_AORUS_PRO_AC },
        new object[] { "B550 AORUS PRO AX", Model.B550_AORUS_PRO_AX },
        new object[] { "B550 VISION D", Model.B550_VISION_D },
        new object[] { "B550 VISION D-P", Model.B550_VISION_D },
        new object[] { "B550 AORUS ELITE", Model.B550_AORUS_ELITE },
        new object[] { "B550 AORUS ELITE V2", Model.B550_AORUS_ELITE },
        new object[] { "B550 AORUS ELITE AX", Model.B550_AORUS_ELITE_AX },
        new object[] { "B550 AORUS ELITE AX V2", Model.B550_AORUS_ELITE_AX },
        new object[] { "B550 AORUS ELITE AX V3", Model.B550_AORUS_ELITE_AX },
        new object[] { "B550 GAMING X", Model.B550_GAMING_X },
        new object[] { "B550 GAMING X V2", Model.B550_GAMING_X },
        new object[] { "B550 UD AC", Model.B550_UD_AC },
        new object[] { "B550 UD AC-Y1", Model.B550_UD_AC },
        new object[] { "B560M AORUS ELITE", Model.B560M_AORUS_ELITE },
        new object[] { "B560M AORUS PRO", Model.B560M_AORUS_PRO },
        new object[] { "B560M AORUS PRO AX", Model.B560M_AORUS_PRO_AX },
        new object[] { "B560I AORUS PRO AX", Model.B560I_AORUS_PRO_AX },
        new object[] { "B650 AORUS ELITE", Model.B650_AORUS_ELITE },
        new object[] { "B650 EAGLE AX", Model.B650_EAGLE_AX },
        new object[] { "B650 AORUS ELITE AX", Model.B650_AORUS_ELITE_AX },
        new object[] { "B650 AORUS ELITE V2", Model.B650_AORUS_ELITE_V2 },
        new object[] { "B650 AORUS ELITE AX V2", Model.B650_AORUS_ELITE_AX_V2 },
        new object[] { "B650 AORUS ELITE AX ICE", Model.B650_AORUS_ELITE_AX_ICE },
        new object[] { "B650 GAMING X AX", Model.B650_GAMING_X_AX },
        new object[] { "B650E AORUS ELITE AX ICE", Model.B650E_AORUS_ELITE_AX_ICE },
        new object[] { "B650M AORUS PRO", Model.B650M_AORUS_PRO },
        new object[] { "B650M AORUS PRO AX", Model.B650M_AORUS_PRO_AX },
        new object[] { "B650M AORUS ELITE", Model.B650M_AORUS_ELITE },
        new object[] { "B650M AORUS ELITE AX", Model.B650M_AORUS_ELITE_AX },
        new object[] { "B650I AX", Model.B650I_AX },
        new object[] { "A620I AX", Model.B650I_AX },
        new object[] { "ROG STRIX Z390-E GAMING", Model.ROG_STRIX_Z390_E_GAMING },
        new object[] { "ROG STRIX Z390-F GAMING", Model.ROG_STRIX_Z390_F_GAMING },
        new object[] { "ROG STRIX Z390-I GAMING", Model.ROG_STRIX_Z390_I_GAMING },
        new object[] { "ROG STRIX Z690-A GAMING WIFI D4", Model.ROG_STRIX_Z690_A_GAMING_WIFI_D4 },
        new object[] { "ROG STRIX Z690-G GAMING WIFI", Model.ROG_STRIX_Z690_G_GAMING_WIFI },
        new object[] { "ROG MAXIMUS XI FORMULA", Model.ROG_MAXIMUS_XI_FORMULA },
        new object[] { "ROG MAXIMUS XII FORMULA", Model.ROG_MAXIMUS_XII_Z490_FORMULA },
        new object[] { "ROG MAXIMUS X HERO (WI-FI AC)", Model.ROG_MAXIMUS_X_HERO_WIFI_AC },
        new object[] { "ROG MAXIMUS Z690 FORMULA", Model.ROG_MAXIMUS_Z690_FORMULA },
        new object[] { "ROG MAXIMUS Z690 HERO", Model.ROG_MAXIMUS_Z690_HERO },
        new object[] { "ROG MAXIMUS Z690 EXTREME GLACIAL", Model.ROG_MAXIMUS_Z690_EXTREME_GLACIAL },
        new object[] { "ROG STRIX X670E-A GAMING WIFI", Model.ROG_STRIX_X670E_A_GAMING_WIFI },
        new object[] { "ROG STRIX X670E-E GAMING WIFI", Model.ROG_STRIX_X670E_E_GAMING_WIFI },
        new object[] { "ROG STRIX X670E-F GAMING WIFI", Model.ROG_STRIX_X670E_F_GAMING_WIFI },
        new object[] { "ROG STRIX B850-E GAMING WIFI", Model.ROG_STRIX_B850_E_GAMING_WIFI },
        new object[] { "ROG STRIX B850-I GAMING WIFI", Model.ROG_STRIX_B850_I_GAMING_WIFI },
        new object[] { "ROG STRIX X870E-E GAMING WIFI", Model.ROG_STRIX_X870E_E_GAMING_WIFI },
        new object[] { "B660GTN", Model.B660GTN },
        new object[] { "X670E VALKYRIE", Model.X670E_Valkyrie },
        new object[] { "ROG MAXIMUS Z790 HERO", Model.ROG_MAXIMUS_Z790_HERO },
        new object[] { "ROG MAXIMUS Z790 DARK HERO", Model.ROG_MAXIMUS_Z790_DARK_HERO },
        new object[] { "PRIME Z690-A", Model.PRIME_Z690_A },
        new object[] { "Z690 Steel Legend WiFi 6E", Model.Z690_Steel_Legend },
        new object[] { "Z690 Steel Legend", Model.Z690_Steel_Legend },
        new object[] { "Z690 Extreme WiFi 6E", Model.Z690_Extreme },
        new object[] { "Z690 Extreme", Model.Z690_Extreme },
        new object[] { "Z790 Pro RS", Model.Z790_Pro_RS },
        new object[] { "Z790 Pro RS WiFi", Model.Z790_Pro_RS },
        new object[] { "Z790 Taichi", Model.Z790_Taichi },
        new object[] { "Z790 Taichi Carrara", Model.Z790_Taichi },
        new object[] { "B650M-C", Model.B650M_C },
        new object[] { "B650M-CW", Model.B650M_C },
        new object[] { "B650M-CX", Model.B650M_C },
        new object[] { "B650M-CWX", Model.B650M_C },
        new object[] { "B650M GAMING PLUS WIFI (MS-7E24)", Model.B650M_Gaming_Plus_Wifi },
        new object[] { "B660 DS3H DDR4-Y1", Model.B660_DS3H_DDR4 },
        new object[] { "B660 DS3H DDR4", Model.B660_DS3H_DDR4 },
        new object[] { "B660 DS3H AC DDR4-Y1", Model.B660_DS3H_AC_DDR4 },
        new object[] { "B660 DS3H AC DDR4", Model.B660_DS3H_AC_DDR4 },
        new object[] { "B660M DS3H AX DDR4", Model.B660M_DS3H_AX_DDR4 },
        new object[] { "MEG X570 UNIFY", Model.X570_MS7C35 },
        new object[] { "MEG X570 UNIFY (MS-7C35)", Model.X570_MS7C35 },
        new object[] { "MEG X570 ACE", Model.X570_MS7C35 },
        new object[] { "MEG X570 ACE (MS-7C35)", Model.X570_MS7C35 },
        new object[] { "ROG STRIX Z790-I GAMING WIFI", Model.ROG_STRIX_Z790_I_GAMING_WIFI },
        new object[] { "ROG STRIX Z790-E GAMING WIFI", Model.ROG_STRIX_Z790_E_GAMING_WIFI },
        new object[] { "ROG STRIX Z790-E GAMING WIFI II", Model.ROG_STRIX_Z790_E_GAMING_WIFI_II },
        new object[] { "MPG X570 GAMING PLUS (MS-7C37)", Model.X570_Gaming_Plus },
        new object[] { "ROG MAXIMUS Z790 FORMULA", Model.ROG_MAXIMUS_Z790_FORMULA },
        new object[] { "Z790 Nova WiFi", Model.Z790_Nova_WiFi },
        new object[] { "ROG MAXIMUS XII HERO (WI-FI)", Model.ROG_MAXIMUS_XII_HERO_WIFI },
        new object[] { "X670E AORUS XTREME", Model.X670E_AORUS_XTREME },
        new object[] { "X870E AORUS PRO", Model.X870E_AORUS_PRO },
        new object[] { "X870E AORUS PRO ICE", Model.X870E_AORUS_PRO_ICE },
        new object[] { "ROG STRIX X870-I GAMING WIFI", Model.ROG_STRIX_X870_I_GAMING_WIFI },
        new object[] { "X870E AORUS XTREME AI TOP", Model.X870E_AORUS_XTREME_AI_TOP },
        new object[] { "PROART X870E-CREATOR WIFI", Model.PROART_X870E_CREATOR_WIFI },
        new object[] { "PRIME X870-P", Model.PRIME_X870_P },
        new object[] { "ROG CROSSHAIR X870E APEX", Model.ROG_CROSSHAIR_X870E_APEX },
        new object[] { "ROG CROSSHAIR X870E HERO", Model.ROG_CROSSHAIR_X870E_HERO },
        new object[] { "ROG CROSSHAIR X870E DARK HERO", Model.ROG_CROSSHAIR_X870E_DARK_HERO },
        new object[] { "MPG Z890 CARBON WIFI (MS-7E17)", Model.Z890_CARBON_WIFI },
        new object[] { "MAG X870E TOMAHAWK WIFI (MS-7E59)", Model.X870E_TOMAHAWK_WIFI },
        new object[] { "MPG X870E CARBON WIFI (MS-7E49)", Model.X870E_CARBON_WIFI },
        new object[] { "MPG Z890 EDGE TI WIFI (MS-7E19)", Model.Z890_EDGE_TI_WIFI },
        new object[] { "X11SWN-E", Model.X11SWN_E },
        new object[] { "PRO B840-P WIFI (MS-7E57)", Model.B840P_PRO_WIFI },
        new object[] { "B840M GAMING PLUS WIFI6E (MS-7E77)", Model.B840M_GAMING_PLUS_WIFI6E },
        new object[] { "B850 GAMING PLUS WIFI6E (MS-7E80)", Model.B850_GAMING_PLUS_WIFI6E },
        new object[] { "PRO B850-P WIFI (MS-7E56)", Model.B850P_PRO_WIFI },
        new object[] { "PRO B850-S WIFI6E (MS-7E80)", Model.B850S_PRO_WIFI6E },
        new object[] { "PRO B850M-A WIFI (MS-7E66)", Model.B850MA_PRO_WIFI },
        new object[] { "PRO B850M-A WIFI PZ (MS-7E78)", Model.B850MA_PRO_WIFI_PZ },
        new object[] { "PRO B850M-P WIFI (MS-7E71)", Model.B850MP_PRO_WIFI },
        new object[] { "B850 GAMING PLUS WIFI (MS-7E56)", Model.B850_GAMING_PLUS_WIFI },
        new object[] { "B850 GAMING PLUS WIFI PZ (MS-7E75)", Model.B850_GAMING_PLUS_WIFI_PZ },
        new object[] { "B850M GAMING PLUS WIFI (MS-7E66)", Model.B850M_GAMING_PLUS_WIFI },
        new object[] { "B850M GAMING PLUS WIFI6E (MS-7E81)", Model.B850M_GAMING_PLUS_WIFI6E },
        new object[] { "MAG B850M MORTAR (MS-7E61)", Model.B850M_MORTAR },
        new object[] { "MAG B850M MORTAR WIFI (MS-7E61)", Model.B850M_MORTAR_WIFI },
        new object[] { "MAG B850 TOMAHAWK WIFI (MS-7E53)", Model.B850_TOMAHAWK_WIFI },
        new object[] { "MAG B850 TOMAHAWK MAX WIFI (MS-7E62)", Model.B850_TOMAHAWK_MAX_WIFI },
        new object[] { "MPG B850 EDGE TI WIFI (MS-7E62)", Model.B850_EDGE_TI_WIFI },
        new object[] { "MPG B850I EDGE TI WIFI (MS-7E79)", Model.B850I_EDGE_TI_WIFI },
        new object[] { "B850MPOWER (MS-7E83)", Model.B850MPOWER },
        new object[] { "X870 GAMING PLUS WIFI (MS-7E47)", Model.X870_GAMING_PLUS_WIFI },
        new object[] { "X870E GAMING PLUS WIFI (MS-7E70)", Model.X870E_GAMING_PLUS_WIFI },
        new object[] { "MAG X870 TOMAHAWK WIFI (MS-7E51)", Model.X870_TOMAHAWK_WIFI },
        new object[] { "MAG X870E TOMAHAWK MAX WIFI PZ (MS-7E84)", Model.X870E_TOMAHAWK_MAX_WIFI_PZ },
        new object[] { "MEG X870E GODLIKE (MS-7E48)", Model.X870E_GODLIKE },
        new object[] { "PRO X870-P WIFI (MS-7E47)", Model.X870P_PRO_WIFI },
        new object[] { "PRO X870E-P WIFI (MS-7E70)", Model.X870EP_PRO_WIFI },
        new object[] { "MPG X870E EDGE TI WIFI (MS-7E59)", Model.X870E_EDGE_TI_WIFI },
        new object[] { "MEG X870E ACE MAX (MS-7E85)", Model.X870E_ACE_MAX },
        new object[] { "MEG Z790 GODLIKE MAX (MS-7D85)", Model.Z790_GODLIKE_MAX },
        new object[] { "MEG Z890 ACE (MS-7E22)", Model.Z890_ACE },
        new object[] { "MEG Z890 UNIFY-X (MS-7E20)", Model.Z890_UNIFY_X },
        new object[] { "MAG Z890 TOMAHAWK WIFI (MS-7E32)", Model.Z890_TOMAHAWK_WIFI },
        new object[] { "MPG Z890I EDGE TI WIFI (MS-7E33)", Model.Z890I_EDGE_TI_WIFI },
        new object[] { "PRO Z890-P WIFI (MS-7E34)", Model.Z890P_PRO_WIFI },
        new object[] { "PRO Z890-A WIFI (MS-7E32)", Model.Z890A_PRO_WIFI },
        new object[] { "PRO Z890-S WIFI (MS-7E54)", Model.Z890S_PRO_WIFI },
        new object[] { "Z890 GAMING PLUS WIFI (MS-7E34)", Model.Z890_GAMING_PLUS_WIFI },
        new object[] { "PRO Z890-S WIFI PZ (MS-7E58)", Model.Z890S_PRO_WIFI_PROJECT_ZERO },
        new object[] { "B850M Steel Legend WiFi", Model.B850M_STEEL_LEGEND_WIFI },
        new object[] { "X870E Taichi", Model.X870E_TAICHI },
        new object[] { "X870E Taichi Lite", Model.X870E_TAICHI },
        new object[] { "X870E Nova WiFi", Model.X870E_NOVA_WIFI },
        new object[] { "X670 AORUS ELITE AX", Model.X670_AORUS_ELITE_AX },
        new object[] { "PROART B760-CREATOR D4", Model.PROART_B760_CREATOR_D4 },
        new object[] { "TUF GAMING B450-PLUS II", Model.TUF_GAMING_B450_PLUS_II },
        new object[] { "FRANBMCP03", Model.FRANBMCP03 },
        new object[] { "FRANBMCP06", Model.FRANBMCP06 },
        new object[] { "FRANBMCP08", Model.FRANBMCP08 },
        new object[] { "FRANBMCP0A", Model.FRANBMCP0A },
        new object[] { "FRANBMCP0B", Model.FRANBMCP0B },
        new object[] { "FRANBMCP0C", Model.FRANBMCP0C },
        new object[] { "FRANGACP04", Model.FRANGACP04 },
        new object[] { "FRANGACP06", Model.FRANGACP06 },
        new object[] { "FRANGACP08", Model.FRANGACP08 },
        new object[] { "FRANMACP04", Model.FRANMACP04 },
        new object[] { "FRANMACP06", Model.FRANMACP06 },
        new object[] { "FRANMACP08", Model.FRANMACP08 },
        new object[] { "FRANMBCP04", Model.FRANMBCP04 },
        new object[] { "FRANMCCP04", Model.FRANMCCP04 },
        new object[] { "FRANMCCP06", Model.FRANMCCP06 },
        new object[] { "FRANMCCP07", Model.FRANMCCP07 },
        new object[] { "FRANMDCP05", Model.FRANMDCP05 },
        new object[] { "FRANMDCP07", Model.FRANMDCP07 },
        new object[] { "FRANMECP02", Model.FRANMECP02 },
        new object[] { "FRANMECP05", Model.FRANMECP05 },
        new object[] { "FRANMECP06", Model.FRANMECP06 },
        new object[] { "FRANMZCP07", Model.FRANMZCP07 },
        new object[] { "FRANMZCP09", Model.FRANMZCP09 },
        new object[] { "FRANMFCP02", Model.FRANMFCP02 },
        new object[] { "FRANMFCP04", Model.FRANMFCP04 },
        new object[] { "FRANMFCP06", Model.FRANMFCP06 },
        new object[] { "FRAPMACP03", Model.FRAPMACP03 },
        new object[] { "FRAPMACP05", Model.FRAPMACP05 },
        new object[] { "FRANMGCP05", Model.FRANMGCP05 },
        new object[] { "FRANMGCP07", Model.FRANMGCP07 },
        new object[] { "FRANMGCP09", Model.FRANMGCP09 },
        new object[] { "Base Board Product Name", Model.Unknown },
        new object[] { "To be filled by O.E.M.", Model.Unknown },    };
  }

  [Theory]
  [MemberData(nameof(ModelCases))]
  public void GetModel_MapsKnownNames_ToExpectedModel(string name, Model expected) {
    Assert.Equal(expected, Identification.GetModel(name));
  }

  // ---------------------------------------------------------------------
  // GetManufacturer — edge cases and matching-strategy behaviour
  // ---------------------------------------------------------------------

  [Fact]
  public void GetManufacturer_UnrecognizedName_ReturnsUnknown() {
    Assert.Equal(Manufacturer.Unknown, Identification.GetManufacturer("Some Totally Unknown Vendor Inc."));
  }

  [Fact]
  public void GetManufacturer_EmptyString_ReturnsUnknown() {
    Assert.Equal(Manufacturer.Unknown, Identification.GetManufacturer(string.Empty));
  }

  [Fact]
  public void GetManufacturer_ToBeFilledByOem_ReturnsUnknown() {
    // SMBIOS placeholder string boards report when no manufacturer is set.
    Assert.Equal(Manufacturer.Unknown, Identification.GetManufacturer("To be filled by O.E.M."));
  }

  [Fact]
  public void GetManufacturer_NullInput_ThrowsNullReferenceException() {
    // The first case (`name.IndexOf(...)`) dereferences `name` directly,
    // so a null input throws before the default branch can be reached.
    // This documents real, current behaviour of the production code.
    Assert.Throws<System.NullReferenceException>(() => Identification.GetManufacturer(null));
  }

  [Theory]
  [InlineData("acer")]
  [InlineData("ACER")]
  [InlineData("AcEr")]
  public void GetManufacturer_StartsWithMatch_IsCaseInsensitive(string name) {
    Assert.Equal(Manufacturer.Acer, Identification.GetManufacturer(name));
  }

  [Theory]
  [InlineData("asrock")]
  [InlineData("ASROCK")]
  [InlineData("AsRock")]
  public void GetManufacturer_EqualsMatch_IsCaseInsensitive(string name) {
    Assert.Equal(Manufacturer.ASRock, Identification.GetManufacturer(name));
  }

  [Theory]
  [InlineData("www.abit.com.tw")]
  [InlineData("prefix-abit.com.tw-suffix")]
  [InlineData("ABIT.COM.TW")]
  public void GetManufacturer_IndexOfMatch_FindsSubstringAnywhereInString(string name) {
    // Unlike the StartsWith-based rules, the abit.com.tw rule uses
    // IndexOf, so it matches even when the marker text isn't at
    // position 0.
    Assert.Equal(Manufacturer.Acer, Identification.GetManufacturer(name));
  }

  [Fact]
  public void GetManufacturer_StartsWithMatch_DoesNotMatchMidString() {
    // "Acer" appears in the string but not as a prefix, and the string
    // doesn't satisfy any other rule, so this should fall through to
    // Unknown rather than incorrectly matching the Acer StartsWith rule.
    Assert.Equal(Manufacturer.Unknown, Identification.GetManufacturer("Distributed by Acer Reseller"));
  }

  [Fact]
  public void GetManufacturer_AsusTekPrefix_MapsToAsus() {
    Assert.Equal(Manufacturer.ASUS, Identification.GetManufacturer("ASUSTeK COMPUTER INC."));
  }

  [Fact]
  public void GetManufacturer_AsusSpacePrefix_MapsToAsus() {
    Assert.Equal(Manufacturer.ASUS, Identification.GetManufacturer("ASUS All Series"));
  }

  // ---------------------------------------------------------------------
  // GetModel — edge cases and matching-strategy behaviour
  // ---------------------------------------------------------------------

  [Fact]
  public void GetModel_UnrecognizedName_ReturnsUnknown() {
    Assert.Equal(Model.Unknown, Identification.GetModel("Some Totally Unknown Board XYZ-9000"));
  }

  [Fact]
  public void GetModel_EmptyString_ReturnsUnknown() {
    Assert.Equal(Model.Unknown, Identification.GetModel(string.Empty));
  }

  [Fact]
  public void GetModel_BaseBoardProductNamePlaceholder_ReturnsUnknown() {
    Assert.Equal(Model.Unknown, Identification.GetModel("Base Board Product Name"));
  }

  [Fact]
  public void GetModel_ToBeFilledByOemPlaceholder_ReturnsUnknown() {
    Assert.Equal(Model.Unknown, Identification.GetModel("To be filled by O.E.M."));
  }

  [Fact]
  public void GetModel_NullInput_ThrowsNullReferenceException() {
    // Same root cause as GetManufacturer: the first case's `when` clause
    // calls an instance method on `name`, so a null input throws before
    // the switch can fall through to default.
    Assert.Throws<System.NullReferenceException>(() => Identification.GetModel(null));
  }

  [Theory]
  [InlineData("p6t")]
  [InlineData("P6T")]
  [InlineData("P6t")]
  public void GetModel_EqualsMatch_IsCaseInsensitive(string name) {
    Assert.Equal(Model.P6T, Identification.GetModel(name));
  }

  [Theory]
  [InlineData("P8P67")]
  [InlineData("P8P67 REV 3.1")]
  public void GetModel_AliasedNames_MapToSameModel(string name) {
    // Two distinct SMBIOS strings are known aliases for the same
    // physical board and must resolve identically.
    Assert.Equal(Model.P8P67, Identification.GetModel(name));
  }

  [Theory]
  [InlineData("B450 AORUS PRO")]
  [InlineData("B450 AORUS PRO WIFI")]
  public void GetModel_WifiVariantAlias_MapsToSameModel(string name) {
    Assert.Equal(Model.B450_AORUS_PRO, Identification.GetModel(name));
  }

  [Fact]
  public void GetModel_TrailingCfSuffixVariant_MapsToBaseModel() {
    // Many Gigabyte boards report a "-CF" suffixed name; this is folded
    // into the same model as the non-suffixed name.
    Assert.Equal(Model.B450M_AORUS_ELITE, Identification.GetModel("B450M AORUS ELITE-CF"));
  }

  [Fact]
  public void GetModel_DifferentCasingOfMsiBracketSuffix_StillMatchesExactly() {
    // GetModel uses exact Equals matching (no substring/prefix), so the
    // full string including the (MS-xxxx) suffix must match exactly.
    Assert.Equal(Model.Z390_GAMING_EDGE_AC, Identification.GetModel("MPG Z390 GAMING EDGE AC (MS-7B17)"));
    Assert.Equal(Model.Unknown, Identification.GetModel("MPG Z390 GAMING EDGE AC"));
  }
}
