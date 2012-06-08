using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Text;

namespace LinkedInFileSearcher
{
	internal static class Program
	{
		private const string PathToLinkedInFile = @"c:\combo_not.txt";

		private static void Main()
		{
			ConsoleKeyInfo consoleKeyInfo;
			Console.Write("Please enter password, keypresses won't be visible: ");


			var passwordStringBuilder = new StringBuilder();

			do
			{
				// hide keypresses
				consoleKeyInfo = Console.ReadKey(true);

				if (consoleKeyInfo.Key == ConsoleKey.Backspace)
				{
					if (passwordStringBuilder.Length <= 0)
					{
						continue;
					}

					passwordStringBuilder.Remove(passwordStringBuilder.Length - 1, 1);
				}

				passwordStringBuilder.Append(consoleKeyInfo.KeyChar);
			}
			while (consoleKeyInfo.Key != ConsoleKey.Enter);

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();

			string pass = passwordStringBuilder.ToString().Trim();

			SHA1 sha1 = SHA1.Create();
			byte[] passwordBytes = Encoding.UTF8.GetBytes(pass);
			byte[] computedHash = sha1.ComputeHash(passwordBytes);

			var stringBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < computedHash.Length; i++)
			{
				stringBuilder.Append(computedHash[i].ToString("x2"));
			}

			string hashedPasswordAsString = stringBuilder.ToString();

			string substringOfPassword = hashedPasswordAsString.Substring(5);

			Console.WriteLine("Searching for" + Environment.NewLine + "{0} or " + Environment.NewLine + "{1} in the database", hashedPasswordAsString, substringOfPassword);

			// using a memory mapped file to not load everything in memory all at once. 
			using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(PathToLinkedInFile))
			{
				Stream memoryMappedViewStream = memoryMappedFile.CreateViewStream();

				using (var sr = new StreamReader(memoryMappedViewStream, Encoding.UTF8))
				{
					while (!sr.EndOfStream)
					{
						String line = sr.ReadLine();
						if (null == line || !line.EndsWith(substringOfPassword))
						{
							continue;
						}

						if (line == hashedPasswordAsString)
						{
							Console.WriteLine("Complete SHA-1 match found: " + line);
						}
						else
						{
							Console.WriteLine("Partial SHA-1 match found: " + line);
						}
					}
				}
			}
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Done... if no matches are found you should be happy.");
			Console.ReadLine();
		}
	}
}