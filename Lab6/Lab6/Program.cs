using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

string thinSeparator = new('-', 105);
string thickSeparator = new('=', 105);
Regex messageRegex = new(@"^[$](.*)[*]([a-fA-F0-9]{2})$");
Dictionary<string, List<string>> dataFieldNames =
	new() {

		{
			"GGA",
			new List<string>() {

				"UTC",
				"Latitude",
				"Latitude Direction",
				"Longitude",
				"Longitude Direction",
				"GPS Quality",
				"SVs",
				"HDOP",
				"Orthometric Height",
				"OH Unit of Measure",
				"Geoid Separation",
				"Data Record Age",
				"Reference Station ID"

			}
		},
		{
			"RMC",
			new List<string>() {

				"UTC",
				"Status",
				"Latitude",
				"Latitude Direction",
				"Longitude",
				"Longitude Direction",
				"Speed (knots)",
				"Track Angle (degrees)",
				"Date",
				"Magnetic Variation (degrees)"

			}
		},
		{
			"VTG",
			new List<string>() {

				"True TMG (degrees)",
				"True TMG Relative To",
				"Magnetic TMG (degrees)",
				"Magnetic TMG Relative To",
				"Speed",
				"Speed Unit of Measure",
				"Speed Over Ground",
				"SOG Unit of Measure",
				"Mode"

			}
		}

	};

Console.WriteLine("Input messages: ");

for(int i = 0; i < args.Length; i++) {

	Console.WriteLine($"  {i + 1}. {args[i]}");

}

Console.WriteLine();
PrintCentered("[PROCESSING STARTED]", false);
Console.WriteLine();

foreach(string message in args) {

	PrintUpperSeparator();
	PrintCentered("NOW PROCESSING", true);
	PrintInnerSeparator();
	PrintCentered(message, true);
	PrintLowerSeparator(true);

	PrintUpperSeparator();

	Match messageRegexMatch = messageRegex.Match(message);

	bool regexFailed = !messageRegexMatch.Success;

	PrintMultiple("Message Validation Result", regexFailed ? "FAILED" : "PASSED");

	if(regexFailed) {

		PrintLowerSeparator(false);

		continue;

	}

	PrintLowerSeparator(true);
	PrintUpperSeparator();

	string content = messageRegexMatch.Groups[1].Value;
	string checksum = messageRegexMatch.Groups[2].Value;

	PrintMultiple("Content", content);
	PrintInnerSeparator();
	PrintMultiple("Checksum", checksum);
	PrintLowerSeparator(true);
	PrintUpperSeparator();
	PrintCentered("CHECKSUM VALIDATION", true);
	PrintInnerSeparator();

	byte computedChecksumByte =
		Encoding.ASCII
				.GetBytes(content)
				.Aggregate((a, b) => (byte) (a ^ b));

	PrintMultiple("Computed Checksum", Convert.ToHexString(new byte[] { computedChecksumByte }));
	PrintInnerSeparator();

	bool checksumValid = computedChecksumByte == Convert.FromHexString(checksum).First();

	PrintMultiple("Validation Status", checksumValid ? "SUCCESS" : "FAIL");

	if(!checksumValid) {

		PrintLowerSeparator(false);

		continue;

	}

	PrintLowerSeparator(true);
	PrintUpperSeparator();
	PrintCentered("DATA FIELDS", true);
	PrintInnerSeparator();

	string[] dataFields = content.Split(',');

	string talkerId = dataFields[0][..2];
	string messageType = dataFields[0][2..];

	Console.WriteLine(new List<string>() {

							"Talker ID",
							"Message Type"

						}.Concat(dataFieldNames[messageType])
							.Zip(new List<string>() {

								talkerId,
								messageType

							}.Concat(dataFields.Skip(1)))
							.Select(dataField =>
										PadMultiple(dataField.First, 30, dataField.Second, 70))
							.Aggregate((a, b) => $"{a}\n+{thinSeparator}+\n{b}"));

	PrintLowerSeparator(false);

	if(args.Last() != message) {

		Console.WriteLine("\n");
		PrintCentered("* * *", false);
		Console.WriteLine("\n");

	}

}

Console.WriteLine();
PrintCentered("[PROCESSING FINISHED]", false);
Console.WriteLine();

void PrintUpperSeparator() => Console.WriteLine($"/{thickSeparator}\\");
void PrintInnerSeparator() => Console.WriteLine($"+{thinSeparator}+");
void PrintLowerSeparator(bool printFollower) {

	Console.WriteLine($"\\{thickSeparator}/");

	if(printFollower) {

		Console.WriteLine($"{PadCenter("+", 107)}\n{PadCenter("|", 107)}");

	}

}

void PrintCentered(string text, bool printBorders) {

	string border = printBorders ? "|" : "";

	Console.WriteLine($"{border} {PadCenter(text, 103)} {border}");

}

void PrintMultiple(string text1, string text2) =>
	Console.WriteLine(PadMultiple(text1, 30, text2, 70));

string PadCenter(string text, int length) =>
	text.PadLeft((length - text.Length) / 2 + text.Length)
		.PadRight(length);
string PadMultiple(string text1, int text1Width, string text2, int text2Width) =>
	$"| {text1.PadRight(text1Width)} | {text2.PadRight(text2Width)} |";
