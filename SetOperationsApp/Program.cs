using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sets;

namespace SetOperations
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Dictionary<string, ISetElement> nameDict = new ();
            List<Set> inputSets = new ();
            Stack<Set> setStack = new (); // for parsing

            while (true)
            {
                Console.Clear();
                Console.WriteLine(
                    "Set Operations\n" +
                    "Input sets must be defined in \"input.txt\"\n\n");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    return;
                }

                // Open file
                if (!File.Exists(ParsingConsts.FilePathInput))
                {
                    File.Create(ParsingConsts.FilePathInput).Dispose();
                }

                string input = File.ReadAllText(ParsingConsts.FilePathInput, Encoding.UTF8);

                // clear everything from the last run
                nameDict.Clear();
                inputSets.Clear();
                setStack.Clear();

                // Parse string
                // Catch anything wrong with input while parsing
                try
                {
                    int i = 0;

                    // while keeping in mind the following:
                    // human poisiton for error display
                    (int line, int col) curPos = (1, 0);

                    // last parsed token
                    byte lastParsedToken = ParsingConsts.ParserTokenNone;

                    // for parsing names
                    bool nameIsBeingParsed = false;
                    bool nameLastParsedUnique = false;
                    int nameStartInd = 0;
                    string nameLastParsed = string.Empty;

                    void NameTryFinishParse()
                    {
                        if (nameIsBeingParsed)
                        {
                            nameIsBeingParsed = false;
                            nameLastParsed = input[nameStartInd..i]; // get substring
                            lastParsedToken = ParsingConsts.ParserTokenName;

                            // add to dictionary if empty
                            nameLastParsedUnique = nameDict.TryAdd(nameLastParsed, new SetValueElem(nameLastParsed));

                            // if parsing set, add to set
                            if (setStack.Count > 0)
                            {
                                Set workSet = setStack.Peek();
                                ISetElement workItem = nameDict[nameLastParsed];
                                if (workItem is Set)
                                {
                                    // recursive set
                                    if (workItem == workSet)
                                    {
                                        throw new InputExceptions($"Set {nameLastParsed} cannot contain itself");
                                    }
                                }

                                workSet.Add(workItem.Dupplicate());
                            }
                        }
                    }

                    bool assignmentFound = false;

                    for (i = 0; i < input.Length; i++)
                    {
                        // go to next line
                        if (input[i] == '\n')
                        {
                            curPos.line++;
                            curPos.col = 0;

                            // since \n is whitespace and we know it is \n
                            NameTryFinishParse();
                            continue;
                        }

                        curPos.col++;

                        // whitespace
                        if (char.IsWhiteSpace(input[i]))
                        {
                            NameTryFinishParse();
                            continue;
                        }

                        // cur char validity cheeck
                        bool isValidChr = false;
                        foreach (var (start, end) in ParsingConsts.ParserAllowedRanges)
                        {
                            if (input[i] >= start && input[i] <= end)
                            {
                                isValidChr = true;
                                break;
                            }
                        }

                        if (!isValidChr && Array.IndexOf(ParsingConsts.ParserAllowedSingles, input[i]) > -1)
                        {
                            isValidChr = true;
                        }

                        if (!isValidChr)
                        {
                            throw new InputExceptions(curPos.line, curPos.col);
                        }

                        // check for symbol
                        switch (input[i])
                        {
                            // =
                            case ParsingConsts.ParserSpcharEquals:
                                {
                                    if (setStack.Count > 0)
                                    {
                                        throw new InputExceptions("Assignment cannot be done inside sets", curPos.line, curPos.col);
                                    }

                                    NameTryFinishParse();
                                    if (lastParsedToken != ParsingConsts.ParserTokenName)
                                    {
                                        throw new InputExceptions("Assignment cannot be done to not a name", curPos.line, curPos.col);
                                    }

                                    if (!nameLastParsedUnique)
                                    {
                                        throw new InputExceptions("Assignment cannot be done to a name that was used before", curPos.line, curPos.col);
                                    }

                                    assignmentFound = true;
                                    lastParsedToken = ParsingConsts.ParserTokenEquals;
                                    break;
                                }

                            // ,
                            case ParsingConsts.ParserSpcharSetSep:
                                {
                                    NameTryFinishParse();
                                    if (lastParsedToken != ParsingConsts.ParserTokenName &&
                                        lastParsedToken != ParsingConsts.ParserTokenSetOrderedEnd &&
                                        lastParsedToken != ParsingConsts.ParserTokenSetUnorderedEnd)
                                    {
                                        throw new InputExceptions("Name or Set Expected", curPos.line, curPos.col);
                                    }

                                    if (setStack.Count == 0)
                                    {
                                        throw new InputExceptions("Separator cannot be outside of a set", curPos.line, curPos.col);
                                    }

                                    lastParsedToken = ParsingConsts.ParserTokenSetSep;
                                    break;
                                }

                            // { <
                            case ParsingConsts.ParserSpcharSetOrderedBegin:
                            case ParsingConsts.ParserSpcharSetUnorderedBegin:
                                {
                                    NameTryFinishParse();

                                    bool isOrdered = input[i] == ParsingConsts.ParserSpcharSetOrderedBegin;
                                    Set nextLayer = new Set(isOrdered);

                                    if (setStack.Count > 0)
                                    {
                                        if (!(lastParsedToken == ParsingConsts.ParserTokenSetSep ||
                                              lastParsedToken == ParsingConsts.ParserTokenSetUnorderedBegin ||
                                              lastParsedToken == ParsingConsts.ParserTokenSetOrderedBegin))
                                        {
                                            throw new InputExceptions(curPos.line, curPos.col);
                                        }

                                        setStack.Peek().Add(nextLayer);
                                        setStack.Push(nextLayer);
                                        lastParsedToken = isOrdered ? ParsingConsts.ParserTokenSetOrderedBegin : ParsingConsts.ParserTokenSetUnorderedBegin;
                                        break;
                                    }

                                    // bottom layer
                                    if (lastParsedToken == ParsingConsts.ParserTokenName)
                                    {
                                        throw new InputExceptions("'=' expected", curPos.line, curPos.col);
                                    }

                                    inputSets.Add(nextLayer);
                                    setStack.Push(nextLayer);
                                    if (assignmentFound)
                                    {
                                        assignmentFound = false;
                                        nameDict[nameLastParsed] = nextLayer;
                                    }

                                    lastParsedToken = isOrdered ? ParsingConsts.ParserTokenSetOrderedBegin : ParsingConsts.ParserTokenSetUnorderedBegin;
                                    break;
                                }

                            // } >
                            case ParsingConsts.ParserSpcharSetUnorderedEnd:
                            case ParsingConsts.ParserSpcharSetOrderedEnd:
                                {
                                    bool isOrdered = input[i] == ParsingConsts.ParserSpcharSetOrderedEnd;
                                    NameTryFinishParse();
                                    if (setStack.Count == 0)
                                    {
                                        throw new InputExceptions("Closing a set that was not opened", curPos.line, curPos.col);
                                    }

                                    if (lastParsedToken == ParsingConsts.ParserTokenSetSep)
                                    {
                                        throw new InputExceptions("Name or Set expected", curPos.line, curPos.col);
                                    }

                                    if (setStack.Peek().IsOrdered)
                                    {
                                        if (!isOrdered)
                                        {
                                            throw new InputExceptions("'>' expected", curPos.line, curPos.col);
                                        }

                                        lastParsedToken = ParsingConsts.ParserTokenSetOrderedEnd;
                                    }
                                    else
                                    {
                                        if (isOrdered)
                                        {
                                            throw new InputExceptions("'}' expected", curPos.line, curPos.col);
                                        }

                                        lastParsedToken = ParsingConsts.ParserTokenSetUnorderedEnd;
                                    }

                                    setStack.Pop();
                                    break;
                                }

                            // name symbol
                            default:
                                if (lastParsedToken == ParsingConsts.ParserTokenName)
                                {
                                    throw new InputExceptions("A name cannot follow another name", curPos.line, curPos.col);
                                }

                                if (lastParsedToken == ParsingConsts.ParserTokenEquals)
                                {
                                    throw new InputExceptions("A name cannot follow '='", curPos.line, curPos.col);
                                }

                                if (!nameIsBeingParsed)
                                {
                                    if (input[i] == '_')
                                    {
                                        throw new InputExceptions("A name cannot begin with an underscore", curPos.line, curPos.col);
                                    }

                                    nameIsBeingParsed = true;
                                    nameStartInd = i;
                                }

                                break;
                        }
                    }

                    // unclosed sets
                    if (setStack.Count > 0)
                    {
                        if (setStack.Peek().IsOrdered)
                        {
                            throw new InputExceptions("> expected near EOF");
                        }

                        /*else*/
                        throw new InputExceptions("} expect near EOF");
                    }

                    // name at the end
                    if (nameIsBeingParsed)
                    {
                        throw new InputExceptions("Unexpected name near EOF");
                    }

                    // there is ordereed set
                    foreach (var item in inputSets)
                    {
                        if (item.IsOrdered)
                        {
                            throw new InputExceptions("Cannot find set symmetric difference with ordered set(s)");
                        }
                    }
                }
                catch (InputExceptions ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("\nPress any key to reset...");
                    Console.ReadKey(true);
                    continue;
                }

                // simple: no exist
                if (inputSets.Count == 0)
                {
                    Console.WriteLine($"For no given sets\n\nThe result of the set symmetric difference is:");
                    Console.WriteLine(Sets.Consts.StrSetUnorderedEmpty);
                    Console.WriteLine("\n\nPress any key to reset...");
                    Console.ReadKey(true);
                    continue;
                }

                Console.WriteLine("Given sets:");
                foreach (var item in inputSets)
                {
                    Console.WriteLine(item);
                }

                // pick operations
                // предусмотреть (без учета кратных вхождений)
                byte choice;
                Console.WriteLine("Доступные операции:\n1 - Объединение(без учёта кратных вхождений)" +
                    "\n2 - Пересечение(без учёта кратных вхождений)" +
                    "\n3 - Симметрическая разность(без учета кратных вхождений)" +
                    "\n4 - Разность двух исходных множеств" +
                    "\n5 - Булеан исходного множества" +
                    "\n6 - Декартово произведение" +
                    "\n7 - Вывести без повторений всевозможные ориентированные множества из элементов исходного неориентированного множества, количество элементов в ориентированном множестве задается n" +
                    "\n8 - Вывести без повторений всевозможные неориентированные множества из элементов исходного неориентированного множества, количество элементов в ориентированном множестве задается n" +
                    "\n9..." +
                    "\n10 - Объединение(с учетом кратных вхождений)" +
                    "\n11 - Пересечение(с учетом кратных вхождений)" +
                    "\n12 - Симметрическая разность(с учетом кратных вхождений)" +
                    "\n13 - Разность(с учетом кратных)");

                Console.WriteLine("Введите номер операции:");
                choice = byte.Parse(Console.ReadLine());
                Set result = new Set(false);
                switch (choice)
                {
                    case 1:
                        result = inputSets[0] + inputSets[1];
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result += inputSets[i];
                        }

                        result = Set.DeleteRepeatingElements(result);
                        break;
                    case 2:
                        result = inputSets[0] % inputSets[1];
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result %= inputSets[i];
                        }

                        result = Set.DeleteRepeatingElements(result);
                        break;
                    case 3:
                        result = inputSets[0] * inputSets[1];
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result += inputSets[i];
                        }

                        result = Set.DeleteRepeatingElements(result);
                        break;
                    case 4:
                        if (inputSets.Count != 2)
                        {
                            Console.WriteLine("Отсутствует необходимое количество множеств.");
                            break;
                        }

                        result = inputSets[0] - inputSets[1];
                        result = Set.DeleteRepeatingElements(result);
                        break;
                    case 5:
                        break;
                    case 6:
                        result = Set.CartesianMultiplication(inputSets[0], inputSets[1]);
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result = Set.CartesianMultiplication(result, inputSets[i]);
                        }

                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        result = inputSets[0] + inputSets[1];
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result += inputSets[i];
                        }

                        break;
                    case 11:
                        result = inputSets[0] % inputSets[1];
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result %= inputSets[i];
                        }

                        break;
                    case 12:
                        result = inputSets[0] * inputSets[1];
                        for (int i = 2; i < inputSets.Count; i++)
                        {
                            result += inputSets[i];
                        }

                        break;
                    case 13:
                        if (inputSets.Count != 2)
                        {
                            Console.WriteLine("Отсутствует необходимое количество множеств.");
                            break;
                        }

                        result = inputSets[0] - inputSets[1];
                        break;
                    default:
                        break;
                }

                // if (inputSets.Count == 1)
                // {
                //    Console.WriteLine($"Only one set given\n\n");
                //    Console.WriteLine("\nPress any key to reset...");
                //    Console.ReadKey(true);
                //    continue;
                // }

                // Output

                Console.WriteLine("\nThe result is: ");
                Console.WriteLine(result);
                Console.WriteLine("\n\nPress any key to reset...");
                Console.ReadKey(true);
            } // continue;
        }
    }
}
