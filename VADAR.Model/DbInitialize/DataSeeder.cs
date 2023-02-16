// <copyright file="DataSeeder.cs" company="VSEC">
// Copyright (c) VSEC. All rights reserved.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using VADAR.Helpers.Enums;

namespace VADAR.Model.DbInitialize
{
    /// <summary>
    /// Data Seeder.
    /// </summary>
    public class DataSeeder
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DataSeeder"/> class.
        /// Data seeder.
        /// </summary>
        public DataSeeder()
        {
        }

        /// <summary>
        /// AddRoleForAdmin.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void AddPermissionsForActionModule(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.InsertData(
               "Permissions",
               new[] { "Id", "Name", "Description", "PermissionType" },
#pragma warning disable SA1118 // Parameter should not span multiple lines
               new object[,]
               {
                    { (int)EnPermissions.AllActionSetting, "AllActionSetting", "All Action Setting", (int)EnPermissionType.System },
                    { (int)EnPermissions.ActionSetting, "ActionSetting", "Action Setting", (int)EnPermissionType.Workspace },
               });
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// DeletePermissionsForActionModule.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void DeletePermissionsForActionModule(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DeleteData(
               "Permissions",
               "Id",
#pragma warning disable SA1118 // Parameter should not span multiple lines
               new object[]
               {
                   (int)EnPermissions.AllActionSetting,
                   (int)EnPermissions.ActionSetting,
               });
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Seeding Data.
        /// </summary>
        /// <param name="migrationBuilder">Migration Builder.</param>
        public void SeedingData(MigrationBuilder migrationBuilder)
        {
#pragma warning disable SA1118 // Parameter must not span multiple lines
            _ = migrationBuilder.InsertData(
                "Countries",
                new[] { "Id", "DisplayName", "Name", "Code" },
                new object[,]
                {
                     { 1,  "Afghanistan",  "Afghanistan",  "AF" },
                     { 2,  "Albania",  "Albania",  "AL" },
                     { 3,  "Algeria",  "Algeria",  "DZ" },
                     { 4,  "AmericanSamoa",  "AmericanSamoa",  "AS" },
                     { 5,  "Andorra",  "Andorra",  "AD" },
                     { 6,  "Angola",  "Angola",  "AO" },
                     { 7,  "Anguilla",  "Anguilla",  "AI" },
                     { 8,  "Antigua and Barbuda",  "Antigua and Barbuda",  "AG" },
                     { 9,  "Argentina",  "Argentina",  "AR" },
                     { 10,  "Armenia",  "Armenia",  "AM" },
                     { 11,  "Aruba",  "Aruba",  "AW" },
                     { 12,  "Australia",  "Australia",  "AU" },
                     { 13,  "Austria",  "Austria",  "AT" },
                     { 14,  "Azerbaijan",  "Azerbaijan",  "AZ" },
                     { 15,  "Bahamas",  "Bahamas",  "BS" },
                     { 16,  "Bahrain",  "Bahrain",  "BH" },
                     { 17,  "Bangladesh",  "Bangladesh",  "BD" },
                     { 18,  "Barbados",  "Barbados",  "BB" },
                     { 19,  "Belarus",  "Belarus",  "BY" },
                     { 20,  "Belgium",  "Belgium",  "BE" },
                     { 21,  "Belize",  "Belize",  "BZ" },
                     { 22,  "Benin",  "Benin",  "BJ" },
                     { 23,  "Bermuda",  "Bermuda",  "BM" },
                     { 24,  "Bhutan",  "Bhutan",  "BT" },
                     { 25,  "Bosnia and Herzegovina",  "Bosnia and Herzegovina",  "BA" },
                     { 26,  "Botswana",  "Botswana",  "BW" },
                     { 27,  "Brazil",  "Brazil",  "BR" },
                     {
                         28,
                         "British Indian Ocean Territory",
                         "British Indian Ocean Territory",
                         "IO",
                     },
                     { 29,  "Bulgaria",  "Bulgaria",  "BG" },
                     { 30,  "Burkina Faso",  "Burkina Faso",  "BF" },
                     { 31,  "Burundi",  "Burundi",  "BI" },
                     { 32,  "Cambodia",  "Cambodia",  "KH" },
                     { 33,  "Cameroon",  "Cameroon",  "CM" },
                     { 34,  "Canada",  "Canada",  "CA" },
                     { 35,  "Cape Verde",  "Cape Verde",  "CV" },
                     { 36,  "Cayman Islands",  "Cayman Islands",  "KY" },
                     {
                         37,
                         "Central African Republic",
                         "Central African Republic",
                         "CF",
                     },
                     { 38,  "Chad",  "Chad",  "TD" },
                     { 39,  "Chile",  "Chile",  "CL" },
                     { 40,  "China",  "China",  "CN" },
                     { 41,  "Christmas Island",  "Christmas Island",  "CX" },
                     { 42,  "Colombia",  "Colombia",  "CO" },
                     { 43,  "Comoros",  "Comoros",  "KM" },
                     { 44,  "Congo",  "Congo",  "CG" },
                     { 45,  "Cook Islands",  "Cook Islands",  "CK" },
                     { 46,  "Costa Rica",  "Costa Rica",  "CR" },
                     { 47,  "Croatia",  "Croatia",  "HR" },
                     { 48,  "Cuba",  "Cuba",  "CU" },
                     { 49,  "Cyprus",  "Cyprus",  "CY" },
                     { 50,  "Czech Republic",  "Czech Republic",  "CZ" },
                     { 51,  "Denmark",  "Denmark",  "DK" },
                     { 52,  "Djibouti",  "Djibouti",  "DJ" },
                     { 53,  "Dominica",  "Dominica",  "DM" },
                     { 54,  "Dominican Republic",  "Dominican Republic",  "DO" },
                     { 55,  "Ecuador",  "Ecuador",  "EC" },
                     { 56,  "Egypt",  "Egypt",  "EG" },
                     { 57,  "El Salvador",  "El Salvador",  "SV" },
                     { 58,  "Equatorial Guinea",  "Equatorial Guinea",  "GQ" },
                     { 59,  "Eritrea",  "Eritrea",  "ER" },
                     { 60,  "Estonia",  "Estonia",  "EE" },
                     { 61,  "Ethiopia",  "Ethiopia",  "ET" },
                     { 62,  "Faroe Islands",  "Faroe Islands",  "FO" },
                     { 63,  "Fiji",  "Fiji",  "FJ" },
                     { 64,  "Finland",  "Finland",  "FI" },
                     { 65,  "France",  "France",  "FR" },
                     { 66,  "French Guiana",  "French Guiana",  "GF" },
                     { 67,  "French Polynesia",  "French Polynesia",  "PF" },
                     { 68,  "Gabon",  "Gabon",  "GA" },
                     { 69,  "Gambia",  "Gambia",  "GM" },
                     { 70,  "Georgia",  "Georgia",  "GE" },
                     { 71,  "Germany",  "Germany",  "DE" },
                     { 72,  "Ghana",  "Ghana",  "GH" },
                     { 73,  "Gibraltar",  "Gibraltar",  "GI" },
                     { 74,  "Greece",  "Greece",  "GR" },
                     { 75,  "Greenland",  "Greenland",  "GL" },
                     { 76,  "Grenada",  "Grenada",  "GD" },
                     { 77,  "Guadeloupe",  "Guadeloupe",  "GP" },
                     { 78,  "Guam",  "Guam",  "GU" },
                     { 79,  "Guatemala",  "Guatemala",  "GT" },
                     { 80,  "Guinea",  "Guinea",  "GN" },
                     { 81,  "Guinea-Bissau",  "Guinea-Bissau",  "GW" },
                     { 82,  "Guyana",  "Guyana",  "GY" },
                     { 83,  "Haiti",  "Haiti",  "HT" },
                     { 84,  "Honduras",  "Honduras",  "HN" },
                     { 85,  "Hungary",  "Hungary",  "HU" },
                     { 86,  "Iceland",  "Iceland",  "IS" },
                     { 87,  "India",  "India",  "IN" },
                     { 88,  "Indonesia",  "Indonesia",  "ID" },
                     { 89,  "Iraq",  "Iraq",  "IQ" },
                     { 90,  "Ireland",  "Ireland",  "IE" },
                     { 91,  "Israel",  "Israel",  "IL" },
                     { 92,  "Italy",  "Italy",  "IT" },
                     { 93,  "Jamaica",  "Jamaica",  "JM" },
                     { 94,  "Japan",  "Japan",  "JP" },
                     { 95,  "Jordan",  "Jordan",  "JO" },
                     { 96,  "Kazakhstan",  "Kazakhstan",  "KZ" },
                     { 97,  "Kenya",  "Kenya",  "KE" },
                     { 98,  "Kiribati",  "Kiribati",  "KI" },
                     { 99,  "Kuwait",  "Kuwait",  "KW" },
                     { 100,  "Kyrgyzstan",  "Kyrgyzstan",  "KG" },
                     { 101,  "Latvia",  "Latvia",  "LV" },
                     { 102,  "Lebanon",  "Lebanon",  "LB" },
                     { 103,  "Lesotho",  "Lesotho",  "LS" },
                     { 104,  "Liberia",  "Liberia",  "LR" },
                     { 105,  "Liechtenstein",  "Liechtenstein",  "LI" },
                     { 106,  "Lithuania",  "Lithuania",  "LT" },
                     { 107,  "Luxembourg",  "Luxembourg",  "LU" },
                     { 108,  "Madagascar",  "Madagascar",  "MG" },
                     { 109,  "Malawi",  "Malawi",  "MW" },
                     { 110,  "Malaysia",  "Malaysia",  "MY" },
                     { 111,  "Maldives",  "Maldives",  "MV" },
                     { 112,  "Mali",  "Mali",  "ML" },
                     { 113,  "Malta",  "Malta",  "MT" },
                     { 114,  "Marshall Islands",  "Marshall Islands",  "MH" },
                     { 115,  "Martinique",  "Martinique",  "MQ" },
                     { 116,  "Mauritania",  "Mauritania",  "MR" },
                     { 117,  "Mauritius",  "Mauritius",  "MU" },
                     { 118,  "Mayotte",  "Mayotte",  "YT" },
                     { 119,  "Mexico",  "Mexico",  "MX" },
                     { 120,  "Monaco",  "Monaco",  "MC" },
                     { 121,  "Mongolia",  "Mongolia",  "MN" },
                     { 122,  "Montenegro",  "Montenegro",  "ME" },
                     { 123,  "Montserrat",  "Montserrat",  "MS" },
                     { 124,  "Morocco",  "Morocco",  "MA" },
                     { 125,  "Myanmar",  "Myanmar",  "MM" },
                     { 126,  "Namibia",  "Namibia",  "NA" },
                     { 127,  "Nauru",  "Nauru",  "NR" },
                     { 128,  "Nepal",  "Nepal",  "NP" },
                     { 129,  "Netherlands",  "Netherlands",  "NL" },
                     { 130,  "Netherlands Antilles",  "Netherlands Antilles",  "AN" },
                     { 131,  "New Caledonia",  "New Caledonia",  "NC" },
                     { 132,  "New Zealand",  "New Zealand",  "NZ" },
                     { 133,  "Nicaragua",  "Nicaragua",  "NI" },
                     { 134,  "Niger",  "Niger",  "NE" },
                     { 135,  "Nigeria",  "Nigeria",  "NG" },
                     { 136,  "Niue",  "Niue",  "NU" },
                     { 137,  "Norfolk Island",  "Norfolk Island",  "NF" },
                     {
                         138,
                         "Northern Mariana Islands",
                         "Northern Mariana Islands",
                         "MP",
                     },
                     { 139,  "Norway",  "Norway",  "NO" },
                     { 140,  "Oman",  "Oman",  "OM" },
                     { 141,  "Pakistan",  "Pakistan",  "PK" },
                     { 142,  "Palau",  "Palau",  "PW" },
                     { 143,  "Panama",  "Panama",  "PA" },
                     { 144,  "Papua New Guinea",  "Papua New Guinea",  "PG" },
                     { 145,  "Paraguay",  "Paraguay",  "PY" },
                     { 146,  "Peru",  "Peru",  "PE" },
                     { 147,  "Philippines",  "Philippines",  "PH" },
                     { 148,  "Poland",  "Poland",  "PL" },
                     { 149,  "Portugal",  "Portugal",  "PT" },
                     { 150,  "Puerto Rico",  "Puerto Rico",  "PR" },
                     { 151,  "Qatar",  "Qatar",  "QA" },
                     { 152,  "Romania",  "Romania",  "RO" },
                     { 153,  "Rwanda",  "Rwanda",  "RW" },
                     { 154,  "Samoa",  "Samoa",  "WS" },
                     { 155,  "San Marino",  "San Marino",  "SM" },
                     { 156,  "Saudi Arabia",  "Saudi Arabia",  "SA" },
                     { 157,  "Senegal",  "Senegal",  "SN" },
                     { 158,  "Serbia",  "Serbia",  "RS" },
                     { 159,  "Seychelles",  "Seychelles",  "SC" },
                     { 160,  "Sierra Leone",  "Sierra Leone",  "SL" },
                     { 161,  "Singapore",  "Singapore",  "SG" },
                     { 162,  "Slovakia",  "Slovakia",  "SK" },
                     { 163,  "Slovenia",  "Slovenia",  "SI" },
                     { 164,  "Solomon Islands",  "Solomon Islands",  "SB" },
                     { 165,  "South Africa",  "South Africa",  "ZA" },
                     {
                         166,
                         "South Georgia and the South Sandwich Islands",
                         "South Georgia and the South Sandwich Islands",
                         "GS",
                     },
                     { 167,  "Spain",  "Spain",  "ES" },
                     { 168,  "Sri Lanka",  "Sri Lanka",  "LK" },
                     { 169,  "Sudan",  "Sudan",  "SD" },
                     { 170,  "Suriname",  "Suriname",  "SR" },
                     { 171,  "Swaziland",  "Swaziland",  "SZ" },
                     { 172,  "Sweden",  "Sweden",  "SE" },
                     { 173,  "Switzerland",  "Switzerland",  "CH" },
                     { 174,  "Tajikistan",  "Tajikistan",  "TJ" },
                     { 175,  "Thailand",  "Thailand",  "TH" },
                     { 176,  "Togo",  "Togo",  "TG" },
                     { 177,  "Tokelau",  "Tokelau",  "TK" },
                     { 178,  "Tonga",  "Tonga",  "TO" },
                     { 179,  "Trinidad and Tobago",  "Trinidad and Tobago",  "TT" },
                     { 180,  "Tunisia",  "Tunisia",  "TN" },
                     { 181,  "Turkey",  "Turkey",  "TR" },
                     { 182,  "Turkmenistan",  "Turkmenistan",  "TM" },
                     {
                         183,
                         "Turks and Caicos Islands",
                         "Turks and Caicos Islands",
                         "TC",
                     },
                     { 184,  "Tuvalu",  "Tuvalu",  "TV" },
                     { 185,  "Uganda",  "Uganda",  "UG" },
                     { 186,  "Ukraine",  "Ukraine",  "UA" },
                     { 187,  "United Arab Emirates",  "United Arab Emirates",  "AE" },
                     { 188,  "United Kingdom",  "United Kingdom",  "GB" },
                     { 189,  "United States",  "United States",  "US" },
                     { 190,  "Uruguay",  "Uruguay",  "UY" },
                     { 191,  "Uzbekistan",  "Uzbekistan",  "UZ" },
                     { 192,  "Vanuatu",  "Vanuatu",  "VU" },
                     { 193,  "Wallis and Futuna",  "Wallis and Futuna",  "WF" },
                     { 194,  "Yemen",  "Yemen",  "YE" },
                     { 195,  "Zambia",  "Zambia",  "ZM" },
                     { 196,  "Zimbabwe",  "Zimbabwe",  "ZW" },
                     { 197,  "land Islands",  "land Islands",  "AX" },
                     { 198,  "Antarctica",  "Antarctica",  "AQ" },
                     {
                         199,
                         "Bolivia, Plurinational State of",
                         "Bolivia, Plurinational State of",
                         "BO",
                     },
                     { 200,  "Brunei Darussalam",  "Brunei Darussalam",  "BN" },
                     { 201,  "Cocos (Keeling) Islands",  "Cocos (Keeling) Islands",  "CC" },
                     { 202,  "Cote d'Ivoire",  "Cote d'Ivoire",  "CI" },
                     { 203,  "Guernsey",  "Guernsey",  "GG" },
                     {
                         204,
                         "Holy See (Vatican City State)",
                         "Holy See (Vatican City State)",
                         "VA",
                     },
                     { 205,  "Hong Kong",  "Hong Kong",  "HK" },
                     {
                          206,
                          "Iran, Islamic Republic of",
                          "Iran, Islamic Republic of",
                          "IR",
                     },
                     { 207,  "Isle of Man",  "Isle of Man",  "IM" },
                     { 208,  "Jersey",  "Jersey",  "JE" },
                     {
                         209,
                         "Korea, Democratic People's Republic of",
                         "Korea, Democratic People's Republic of",
                         "KP",
                     },
                     { 210,  "Korea, Republic of",  "Korea, Republic of",  "KR" },
                     {
                         211,
                         "Lao People's Democratic Republic",
                         "Lao People's Democratic Republic",
                         "LA",
                     },
                     { 212,  "Libyan Arab Jamahiriya",  "Libyan Arab Jamahiriya",  "LY" },
                     { 213,  "Macao",  "Macao",  "MO" },
                     {
                         214,
                         "Macedonia, The Former Yugoslav Republic of",
                         "Macedonia, The Former Yugoslav Republic of",
                         "MK",
                     },
                     {
                         215,
                         "Micronesia, Federated States of",
                         "Micronesia, Federated States of",
                         "FM",
                     },
                     { 216,  "Moldova, Republic of",  "Moldova, Republic of",  "MD" },
                     { 217,  "Mozambique",  "Mozambique",  "MZ" },
                     {
                         218,
                         "Palestinian Territory, Occupied",
                         "Palestinian Territory, Occupied",
                         "PS",
                     },
                     { 219,  "Pitcairn",  "Pitcairn",  "PN" },
                     { 220,  "Reunion",  "Reunion",  "RE" },
                     { 221,  "Russia",  "Russia",  "RU" },
                     { 222,  "Saint Barthélemy",  "Saint Barthélemy",  "BL" },
                     {
                         223,
                         "Saint Helena, Ascension and Tristan Da Cunha",
                         "Saint Helena, Ascension and Tristan Da Cunha",
                         "SH",
                     },
                     { 224,  "Saint Kitts and Nevis",  "Saint Kitts and Nevis",  "KN" },
                     { 225,  "Saint Lucia",  "Saint Lucia",  "LC" },
                     { 226,  "Saint Martin",  "Saint Martin",  "MF" },
                     {
                         227,
                         "Saint Pierre and Miquelon",
                         "Saint Pierre and Miquelon",
                         "PM",
                     },
                     {
                         228,
                         "Saint Vincent and the Grenadines",
                         "Saint Vincent and the Grenadines",
                         "VC",
                     },
                     { 229,  "Sao Tome and Principe",  "Sao Tome and Principe",  "ST" },
                     { 230,  "Somalia",  "Somalia",  "SO" },
                     { 231,  "Svalbard and Jan Mayen",  "Svalbard and Jan Mayen",  "SJ" },
                     { 232,  "Syrian Arab Republic",  "Syrian Arab Republic",  "SY" },
                     {
                         233,
                         "Taiwan, Province of China",
                         "Taiwan, Province of China",
                         "TW",
                     },
                     {
                         234,
                         "Tanzania, United Republic of",
                         "Tanzania, United Republic of",
                         "TZ",
                     },
                     { 235,  "Timor-Leste",  "Timor-Leste",  "TL" },
                     {
                         236,
                         "Venezuela, Bolivarian Republic of",
                         "Venezuela, Bolivarian Republic of",
                         "VE",
                     },
                     { 237,  "Viet Nam",  "Viet Nam",  "VN" },
                     { 238,  "Virgin Islands, British",  "Virgin Islands, British",  "VG" },
                     { 239,  "Virgin Islands, U.S.",  "Virgin Islands, U.S.",  "VI" },
                     {
                         240,
                         "Congo, The Democratic Republic of the",
                         "Congo, The Democratic Republic of the",
                         "CD",
                     },
                     {
                         241,
                         "Falkland Islands (Malvinas)",
                         "Falkland Islands (Malvinas)",
                         "FK",
                     },
                });

            _ = migrationBuilder.InsertData(
                "Users",
                new[] { "Id", "IsProfileUpdated", "Email", "UserName", "CountryId", "Status", "JoinDate" },
                new object[,]
                {
                    { "a90ba705-1426-42ec-8d68-3f252d4f9095",  true, "giabao_pham1982vn@yahoo.com", "admin", 85, 0, DateTime.UtcNow },
                });

            _ = migrationBuilder.InsertData(
               "Permissions",
               new[] { "Id", "Name", "Description", "PermissionType" },
               new object[,]
               {
                    { 1,  "Host Setting", "Configure host permission.", 0 },
                    { 2,  "Host View", "View Host Information.", 0 },
                    { 3,  "Group Setting", "Configure Group Permission.", 0 },
                    { 4,  "Group View", "View Group Permission.", 0 },
                    { 5,  "Dashboard View", "View Dashboard Permission.", 0 },
                    { 6,  "Logs View", "View Logs Permission.", 0 },
                    { 7,  "Events View", "View Events Permission.", 0 },
                    { 8,  "Notification View", "View Notification Permission.", 0 },
                    { 9,  "All Hosts Setting", "Configure host permission.", 1 },
                    { 10,  "All Hosts View", "View Host Information.", 1 },
                    { 11,  "All Groups Setting", "Configure Group Permission.", 1 },
                    { 12,  "All Groups View", "View Group Permission.", 1 },
                    { 13,  "All Dashboards View", "View Dashboard Permission.", 1 },
                    { 14,  "All Logs View", "View Logs Permission.", 1 },
                    { 15,  "All Events View", "View Events Permission.", 1 },
                    { 16,  "All Notification View", "View Notification Permission.", 1 },
                    { 17,  "Permission Setting", "Permission Setting Permission.", 1 },
                    { 18,  "Role Setting", "Role Setting Permission.", 1 },
                    { 19,  "User Setting", "User Setting Permission.", 1 },
                    { 20,  "Workspace Role Setting", "Workspace Role Setting Permission.", 0 },
                    { 21,  "Workspace Role Permission Setting", "Workspace Role Permission Setting Permission.", 0 },
                    { 22,  "Workspace Role User Setting", "Workspace Setting Permission.", 0 },
                    { 10000,  "System full permission", "System full permission.", 1 },
                    { 10001,  "Workspace full permission", "Workspace full permission.", 0 },
               });

            // _ = migrationBuilder.InsertData(
            //    table: "WorkspaceRoles",
            //    columns: new[] { "Id", "Name", "Description" },
            //    values: new object[,]
            //    {
            //        { "a10ba705-1426-42ec-8d68-3f252d4f1015", "Workspace Admin", "Admin permission of the workspace." },
            //        { "a20ba705-1426-42ec-8d68-3f252d4f2025", "Workspace User", "User permission of the workspace." },
            //    });

            // _ = migrationBuilder.InsertData(
            //    table: "WorkspaceRolePermissions",
            //    columns: new[] { "PermissionId", "WorkspaceRoleId" },
            //    values: new object[,]
            //    {
            //        { 1, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 2, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 3, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 4, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 5, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 6, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 7, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 8, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 20, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 21, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 22, "a10ba705-1426-42ec-8d68-3f252d4f1015" },
            //        { 2, "a20ba705-1426-42ec-8d68-3f252d4f2025" },
            //        { 4, "a20ba705-1426-42ec-8d68-3f252d4f2025" },
            //        { 5, "a20ba705-1426-42ec-8d68-3f252d4f2025" },
            //        { 6, "a20ba705-1426-42ec-8d68-3f252d4f2025" },
            //        { 7, "a20ba705-1426-42ec-8d68-3f252d4f2025" },
            //        { 8, "a20ba705-1426-42ec-8d68-3f252d4f2025" },
            //    });
            _ = migrationBuilder.InsertData(
               "Roles",
               new[] { "Id", "Name" },
               new object[,]
               {
                    { "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6",  "Admin" },
                    { "52f69311-ac69-4959-a6a5-5cd0f81ccbc4",  "User" },
               });

            _ = migrationBuilder.InsertData(
                "RolePermissions",
                new[] { "PermissionId", "RoleId" },
                new object[,]
                {
                    { 9, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 10, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 11, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 12, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 13, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 14, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 15, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 16, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 17, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 18, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 19, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                });
#pragma warning restore SA1118 // Parameter must not span multiple lines
        }

        /// <summary>
        /// AddRoleForAdmin.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void AddRoleForAdmin(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.InsertData(
               "RoleUsers",
               new[] { "UserId", "RoleId" },
#pragma warning disable SA1118 // Parameter should not span multiple lines
               new object[,]
               {
                    { "a90ba705-1426-42ec-8d68-3f252d4f9095", "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { "a90ba705-1426-42ec-8d68-3f252d4f9095", "52f69311-ac69-4959-a6a5-5cd0f81ccbc4" },
                    { "a90ba705-1426-42ec-8d68-3f252d4f9095", "2248046b-ebd3-47f5-8ac5-9db158fa9cb3" },
               });
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Seeding Language.
        /// </summary>
        /// <param name="migrationBuilder">Migration Builder.</param>
        public void LanguageSeeding(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.InsertData(
                "Languages",
                new[] { "Id", "Name", "Code" },
#pragma warning disable SA1118 // Parameter must not span multiple lines
                new object[,]
                {
                    { 1, "English", "en-us" },
                    { 2, "Vietnamese", "vi-vn" },
                });
#pragma warning restore SA1118 // Parameter must not span multiple lines
        }

        /// <summary>
        /// Seeding Language.
        /// </summary>
        /// <param name="migrationBuilder">Migration Builder.</param>
        public void PolicySeeding(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql("DELETE FROM Policies", true);
            _ = migrationBuilder.InsertData(
                "Policies",
                new[] { "Id", "Description" },
#pragma warning disable SA1118 // Parameter must not span multiple lines
                new object[,]
                {
                    { 1, "High amount of POST requests in a small period of time (likely bot)." },
                    { 2, "Multiple web server 503 error code (Service unavailable)." },
                    { 3, "Multiple web server 400 error codes from same source ip." },
                    { 4, "Multiple SQL injection attempts from same source ip." },
                    { 5, "Multiple web server 500 error code (Internal Error)." },
                    { 6, "sshd: brute force trying to get access to the system." },
                    { 7, "PAM: Multiple failed logins in a small period of time." },
                    { 8, "sshd: Multiple authentication failures." },
                    { 9, "syslog: User missed the password more than one time." },
                    { 10, "Apache: Multiple Invalid URI requests from same source." },
                    { 11, "Multiple XSS (Cross Site Scripting) attempts from same source ip." },
                    { 12, "Multiple common web attacks from same source ip." },
                });
#pragma warning restore SA1118 // Parameter must not span multiple lines
        }

        /// <summary>
        /// PermissionsSeeding.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void PermissionsSeeding(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.Sql("DELETE FROM Permissions", true);

            _ = migrationBuilder.InsertData(
               "Permissions",
               new[] { "Id", "Name", "Description", "PermissionType" },
#pragma warning disable SA1118 // Parameter must not span multiple lines
               new object[,]
               {
                    { 1,  "Host Setting", "Configure host permission.", 0 },
                    { 2,  "Host View", "View Host Information.", 0 },
                    { 3,  "Group Setting", "Configure Group Permission.", 0 },
                    { 4,  "Group View", "View Group Permission.", 0 },
                    { 5,  "Workspace Permission View", "View Permission.", 0 },
                    { 6,  "Workspace Permisison Setting", "Configure Permission.", 0 },
                    { 7,  "Email Notification View", "View Email Notification Permission.", 0 },
                    { 8,  "Email Notification Setting", "Configure Email Notification Permission.", 0 },
                    { 9,  "Policy View", "View Policy Permission.", 0 },
                    { 10, "Policy Setting", "Configure Policy Permission.", 0 },
                    { 11, "Whitelist Ip View", "View Whitelist Ip Permission.", 0 },
                    { 12, "Whitelist Ip Setting", "Configure Whitelist Ip Permission.", 0 },
                    { 13, "Events View", "View Events Permission.", 0 },
                    { 14, "Logs View", "View Logs Permission.", 0 },
                    { 15, "Dashboard View", "View Dashboard Permission.", 0 },
                    { 16,  "All Hosts Setting", "Configure host permission.", 1 },
                    { 17,  "All Hosts View", "View Host Information.", 1 },
                    { 18,  "All Groups Setting", "Configure Group Permission.", 1 },
                    { 19,  "All Groups View", "View Group Permission.", 1 },
                    { 20,  "All Dashboards View", "View Dashboard Permission.", 1 },
                    { 21,  "All Logs View", "View Logs Permission.", 1 },
                    { 22,  "All Events View", "View Events Permission.", 1 },
                    { 23,  "All Notification View", "View Notification Permission.", 1 },
                    { 24,  "All Permission Setting", "Permission Setting Permission.", 1 },
                    { 25,  "All Role Setting", "Role Setting Permission.", 1 },
                    { 26,  "All User Setting", "User Setting Permission.", 1 },
                    { 10000,  "System full permission", "System full permission.", 1 },
               });

            _ = migrationBuilder.InsertData(
                "RolePermissions",
                new[] { "PermissionId", "RoleId" },
                new object[,]
                {
                    { 1, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 2, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 3, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 4, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 5, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 6, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 7, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 8, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 9, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 10, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 11, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 12, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 13, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 14, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 15, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                    { 10000, "b78d4f06-34fa-4a5f-bd5d-2b28288e2cc6" },
                });
        }

        /// <summary>
        /// SeedDataForAgentOsModule.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void SeedDataForAgentOsModule(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.InsertData(
               "AgentOs",
               new[] { "Id", "Icon", "Name", "Description" },
#pragma warning disable SA1118 // Parameter should not span multiple lines
               new object[,]
               {
                    { 1, "https://dev.admin.vadar.vn/assets/images/host/ubuntu.svg", "Ubuntu", "Ubuntu" },
                    { 2, "https://dev.admin.vadar.vn/assets/images/host/windows.svg", "Window", "Window" },
                    { 3, "https://dev.admin.vadar.vn/assets/images/host/mac.svg", "MacOs", "MacOs" },
                    { 4, "https://dev.admin.vadar.vn/assets/images/host/centos.svg", "Centos", "Centos" },
               });
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// SeedDataForAgentInstallModule.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void SeedDataForAgentInstallModule(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.InsertData(
               "AgentInstalls",
               new[] { "Id", "OsId", "Version", "Type", "Status" },
#pragma warning disable SA1118 // Parameter should not span multiple lines
               new object[,]
               {
                    { 1, 1, "U18-1.0.0", 1, 1 },
                    { 2, 1, "U20-1.0.0", 1, 1 },
                    { 3, 1, "1.0.0", 2, 1 },
                    { 4, 4, "1.0.0", 2, 1 },
               });
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// SeedDataForAgentInstallModule.
        /// </summary>
        /// <param name="migrationBuilder">migrationBuilder.</param>
        public void ClaimSeedingForFirstClickHost(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.InsertData(
                "Claims",
                new[] { "Id", "Name" },
#pragma warning disable SA1118 // Parameter must not span multiple lines
                new object[,]
                {
                    { 1, "FirstClickHost" },
                });
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }
    }
}
