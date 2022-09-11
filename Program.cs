using System;
using System.Diagnostics;

internal class Program
{
	/*Summering hemläxa vecka 2
	Eftersom utmaningen i detta projekt var att inte använda klasser behöver vi tilldela
	vissa variabler direkt vid programstart.
	Dessa variabler är här för att vi skall slippa passa dem som referenser från olika klasser.
	Scopet är alltså globalt.
	
	Till Läraren:
	Jag har delat upp kod i metoder med så tydliga namn som möjligt så att man enkelt kan hoppa
	över repetitionskod. Detta är för att jag implementerat alla extrauppgifter och därav korsat
	gränsen för 50 rader kod.
	Jag vet att filen ser gigantisk ut (och det är den) men det är mycket hemmagjorda boiler grejer.
	
	Koden är också uppdelad i 4st regions för att öka läsbarheten:
	1: Main Menu -> Inehåller funktioner för huvudmenyn samt ett drawcall för att rita ut denne.
	2: User Registry -> Inehåller alla metoder som påverkar eller använder adressboken alltså ""databasen"".
	3: Input -> Inehåller alla metoder för att påverka det globala input tillståndet.
	4: Draw -> Inehåller alla generella metoder för att "spicea up" UX designen. eg: DrawHeader, DrawLine
	
	Om du tappar bort dig så hårt att du inte ens orkar skumma igenom min kod har jag misslyckats med vad jag försökt.
	
	Självreflektion:
	Att skriva ett helt program utan att dela upp det i klasser eller använda listor var en utmaning
	och jag har behövt att applicera ett helt nytt sätt att se på min kodstruktur under detta projekt.
	I början tyckte jag det var jobbigt och i all ärlighet onödigt / elakt att lägga till det kravet,
	men efter ett tag blev denna typen av problemlösning riktigt rolig och omställande från vad jag är van vid.
	Överlag är jag nöjd med uppgiften och hur jag löste den.
	*/
	
	/*Uppdattering:
	Uppdaterat programmet så att slutanvändaren inte kan spara namn med massa blanksteg.
	Ändrat metoden för att rita en linje med en valfri offset för terminalpekaren.
	Ändrade if satsen i GetKeyDown så att man returnerar kontrollen direkt istället för att kontrollera den i en ifsats.
	*/
	
	// Global scope UI colors
	static ConsoleColor elementColor = ConsoleColor.White;
	static ConsoleColor selectedElementColor = ConsoleColor.Green;

	// Global scope input system
	static ConsoleKey inputKey;
	static int selectedElement = 0;
	static int maxElementIndex = 0;
	static bool confirmElement = false;
	
	// Global scope stats
	static string biggestNameString = string.Empty;
	static int biggestNameInt = 0;
	static int totalCharacters = 0;
	
	// Global scope konami
	static ConsoleKey[] konamiSequence = new ConsoleKey[] 
	{ConsoleKey.UpArrow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.B, ConsoleKey.A,}; 
	static int currentKonamiIndex = 0;
	
	// Global scope User "database"
	// Aka : Adress book
	static string[]? userRegistry = new string[0];
	
	public static void Main(string[] args)
	{
		// This while loop is needed to rerun the project.
		// The system is agile though, so we could remove the while loop and add as many UIs as we want.
		while (true)
			MenuLoop();
	}

	#region Main menu
	// Collect code for the mainframe loop into 1 method.
	static void MenuLoop()
	{
		// Reset global input values
		// This is what makes us able to expand the project to infinity.
		ResetInput();
		
		while (!confirmElement)
		{
			DrawMenuUI();
			inputKey = Console.ReadKey().Key;
			UpdateInput();
		}
		
		// The global inputsystem has detected a button press and is storing the button in selectedElement
		switch (selectedElement)
		{
			case 0:
				AddUserLoop();
				break;
			case 1:
				ListUserArray();
				break;
			case 2:
				ClearUserArrayLoop();
				break;
			case 3:
				ListUserStats();
				break;
			case 4:
				Environment.Exit(0);
				break;
		}
	}

	static void DrawMenuUI()
	{
		Console.ForegroundColor = elementColor;
		Console.Clear();
		DrawHeader("Adressbok");
		SelectableElement(0, "Lägg till");
		Console.WriteLine();
		SelectableElement(1, "Visa alla");
		Console.WriteLine();
		SelectableElement(2, "Rensa lista");
		Console.WriteLine();
		SelectableElement(3, "Statistik");
		Console.WriteLine();
		SelectableElement(4, "Avsluta");
		DrawLine();
	}
	#endregion

	#region User Registry
	
	// The wrapper frame for adding a user.
	static void AddUserLoop()
	{
		// Define header as we will need it in lower scopes.
		string h = "Adressbok - Lägg till ny";
		
		// Force the end-user to input a valid name.
		// GetInputName will not stop looping until a valid name is given. (muahaha)
		Console.Clear();
		string newUser = GetInputName(h);
		Console.Clear();
		
		// Expand the userRegistry with the valid name given.
		DrawHeader(h);
		AddUserToArray(newUser);
		
		// Confirm that we have added the name.
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"{newUser} har lagts till i adressboken.");
		Console.ForegroundColor = elementColor;
		
		DrawLine();
		
		WaitForAnyKey();
	}
	
	// Expand the userRegistry using a temp array.
	static void AddUserToArray(string newUser)
	{
		string[] tempUsers = new string[userRegistry.Length+1];
		for(int i = 0; i < tempUsers.Length; i++)
		{
			if(i < userRegistry.Length)
				tempUsers[i] = userRegistry[i];
			else
				tempUsers[i] = newUser;
		}
		userRegistry = new string[tempUsers.Length];
		tempUsers.CopyTo(userRegistry, 0);
		
		// Compile global stats after every name added.
		CompileStats();
	}
	
	// Enter the UI for clearing the userRegistry.
	static void ClearUserArrayLoop()
	{
		// Using the global input system, requires reset.
		ResetInput();
		
		while (!confirmElement)
		{
			DrawClearUserUI();
			inputKey = Console.ReadKey().Key;
			UpdateInput();
		}
		
		switch(selectedElement)
		{
			case 0:
				userRegistry = new string[0];
				ResetStats();
				break;
			case 1:
				break;
		}
	}
	
	static void DrawClearUserUI()
	{
		Console.Clear();
		DrawHeader("Adressbok - Rensa");
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("Är du säker på att du vill rensa listan?\nDetta går inte att ångra.");
		Console.ForegroundColor = elementColor;
		Console.WriteLine();
		SelectableElement(0, "Rensa lista");
		Console.WriteLine();
		SelectableElement(1, "Avbryt");
		DrawLine();
	}
	
	static void ResetStats()
	{
		biggestNameInt = 0;
		biggestNameString = string.Empty;
		totalCharacters = 0;
	}
	
	static void CompileStats()
	{
		// Reset the stats before every compilation so we don't need to worry from
		// where the compile call was made.
		ResetStats();
		
		for(int i = 0; i < userRegistry.Length; i++)
		{
			if(userRegistry[i].Length > biggestNameInt)
			{
				biggestNameInt = userRegistry[i].Length;
				biggestNameString = userRegistry[i];
			}
			
			totalCharacters += userRegistry[i].Length;
		}
	}
	
	// If the userRegistry is empty, show warning.
	// Else show stats.
	static void ListUserStats()
	{
		Console.Clear();
		DrawHeader("Adressbok - Statistik");
		if(userRegistry.Length <= 0)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Statistik otillgänglig - Adressboken är tom.");
			Console.ForegroundColor = elementColor;
		}
		else
		{	
			Console.WriteLine($"Antal namn:\t\t\t\t{userRegistry.Length}");
			Console.WriteLine($"Största namnet:\t\t\t\t{biggestNameInt} ({biggestNameString})");
			Console.WriteLine($"Genomsnittlig storlek:\t\t\t{totalCharacters / userRegistry.Length}");
			Console.WriteLine($"Totalt antal tecken:\t\t\t{totalCharacters}");
		}
		DrawLine();
		WaitForAnyKey();
	}
	
	// List all the users in the userRegistry
	// If the list is empty no names will show up.
	static void ListUserArray()
	{
		Console.Clear();
		DrawHeader("Adressbok - Lista");
		for(int i = 0; i < userRegistry.Length; i++)
		{
			Console.WriteLine($"{i+1} : {userRegistry[i]}");
		}
		DrawLine();
		WaitForAnyKey();
	}
	
	// Hacky method for forcing the user to input a valid name with the following criteria:
	// Not empty
	// Only letters (and spaces)
	// At least 2 letters
	// If criteria is not met: Show error and have the end-user retry.
	static string GetInputName(string header)
	{
		string errors = string.Empty;
		bool failed = false;
		
		// Local scope method.
		// Hacky, but whatever.
		void ThrowNameError(string message)
		{
			errors += message + "\n";
			failed = true;
		}
		
		while (true)
		{
			int letters = 0;
			failed = false;
			
			Console.Clear();
			DrawHeader(header);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(errors);
			Console.ForegroundColor = elementColor;
			
			DrawLine(2);
			Console.Write("Namn: ");
			string inputString = Console.ReadLine();
			errors = string.Empty;
			
			char lastChar = 'a';
			
			if (inputString.Length > 0)
			{
				foreach (char c in inputString)
				{
					if (!char.IsLetter(c) && c != ' ')
					{
						ThrowNameError("Namn kan bara inehålla bokstäver.");
						break;
					}
					if (char.IsLetter(c))
						letters++;
						
					if(c == ' ' && lastChar == ' ')
					{
						ThrowNameError("Namn kan inte ha flera blanksteg i rad.");
						break;
					}
					
					lastChar = c;
				}
				if (letters < 2)
					ThrowNameError("Namn måste inehålla minst 2 bokstäver.");
				
				if(inputString[0] == ' ' || inputString[inputString.Length-1] == ' ')
					ThrowNameError("Namn kan inte börja eller sluta med blanksteg.");
			}
			else
				ThrowNameError("Du måste skriva ett namn.");
				
			if (!failed)
				return inputString;
		}
	}
	#endregion

	#region Input
	// The global input system
	// This uses global scope variables to send input states.
	static void UpdateInput()
	{
		// Ignore this.
		CheckKonami();
		
		if (GetKeyDown(ConsoleKey.UpArrow))
		{
			if (selectedElement - 1 < 0)
				selectedElement = maxElementIndex;
			else
				selectedElement--;
		}
		if (GetKeyDown(ConsoleKey.DownArrow))
		{
			if (selectedElement + 1 > maxElementIndex)
				selectedElement = 0;
			else
				selectedElement++;
		}
		if(GetKeyDown(ConsoleKey.Enter) || GetKeyDown(ConsoleKey.Spacebar))
			confirmElement = true;
	}
	
	// Resets the global input variables
	// This is needed to initialize new menu elements
	static void ResetInput()
	{
		confirmElement = false;
		selectedElement = 0;
		maxElementIndex = 0;
	}
	
	// Pause the application until the user presses enter
	// The user can input text and whatnot, but that wont affect the program as we don't cache their input.
	static void WaitForAnyKey(string message = "Tryck på valfri tangent för att fortsätta.")
	{
		Console.WriteLine(message);
		Console.ReadKey();
	}
	
	// Part of the global input system.
	// Checks if the key sent by the input system is the parameter.
	static bool GetKeyDown(ConsoleKey key)
	{
		return (inputKey == key);
	}
	
	// Create an element that uses the global input system to update its color and formating.
	// When these are created, they will add themselves to the global index.
	// This is why we need to reset the system everytime we enter a new menu.
	// Had we used OOP this would not be necessary.
	static void SelectableElement(int index, string message, bool newLine = true)
	{
		string spacing = string.Empty;

		if (index > maxElementIndex)
			maxElementIndex = index;
		if (selectedElement == index)
		{
			Console.ForegroundColor = selectedElementColor;
			spacing = " ";
		}
		if (newLine)
			Console.WriteLine($">{spacing}{message}");
		else
			Console.Write($">{spacing}{message}");

		Console.ForegroundColor = elementColor;
	}
	
	#endregion

	#region Konami
	static void CheckKonami()
	{
		if(GetKeyDown(konamiSequence[currentKonamiIndex]))
			currentKonamiIndex++;
		else
			currentKonamiIndex = 0;
			
		if(currentKonamiIndex >= 10)
		{
			currentKonamiIndex = 0;
			RunKonami();
		}
	}
	
	static void RunKonami()
	{
		System.Diagnostics.Process.Start(new ProcessStartInfo("https://www.youtube.com/watch?v=dQw4w9WgXcQ") {UseShellExecute = true});
	}
	
	#endregion

	#region Draw
	// Surround the parameter message with a nice header.
	// This is it's own method so we can easily change all headers on a global scale.
	static void DrawHeader(string message)
	{
		DrawLine();
		Console.WriteLine(message);
		DrawLine();
	}
	
	// Draw a line.
	// This is it's own method so we can easily change all lines on a global scale.
	// We also include some magic cursor manipulation so we can place a ReadLine above the line.
	// Brain = Big
	static void DrawLine(int cursorOffset = 0)
	{
		Console.CursorTop += cursorOffset;
		Console.WriteLine("--------------------------------------------------");
		Console.CursorTop -= cursorOffset;
	}
	#endregion
}