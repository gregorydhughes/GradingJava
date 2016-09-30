using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using nmspcException;


namespace GradingJava
{
    class Program
    {
        static public List<string> sectionMembers;

        static public string javaProgramName;
        static public string masterDirectory;

        static void Main(string[] args)
        {
            SetWorkingDirectoryAndWorkName();

            ReadAllMembersOfSection();

            Console.Clear();

            Console.WriteLine("All set!\n");
                     
            foreach (string currMem in sectionMembers)
            {
                string question = "Would you like to check " + currMem + "'s submission of " + javaProgramName + "?";

                if (GetBooleanResponse(question))
                {
                    Console.Clear();
                    Console.WriteLine("Checking " + currMem + "'s submission of " + javaProgramName);

                    try
                    {
                        Directory.SetCurrentDirectory(masterDirectory + "\\" + currMem);

                        string realName = javaProgramName;

                        bool exit = false;

                        while (!File.Exists(Directory.GetCurrentDirectory().ToString() + "\\" + realName + ".java") && !exit)
                        {
                            realName = ReturnCorrectedJavaName();

                            if (!File.Exists(Directory.GetCurrentDirectory().ToString() + "\\" + realName + ".java"))
                            {
                                if (GetBooleanResponse("Would you like to move on to the next student?"))
                                    exit = true;
                            }
                        }

                        if (!exit)
                        {
                            CompileAndRunJavaProgram(realName);

                            string nextMove;

                            if (currMem == sectionMembers.Last())
                                nextMove = " exit...";
                            else
                                nextMove = " move on...";

                            Console.WriteLine("\n\nPress any key to" + nextMove);
                            Console.ReadKey();
                        }
                    }
                    catch (NotAllThatBadException ne)
                    {
                        // Who Cares!
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString() + "\nError in Java processing...\nFuck Java\nPress any key to exit...");
                        Console.ReadKey();
                    }

                    // Reset to the master working directory
                    Directory.SetCurrentDirectory(masterDirectory);
                }

                //else Do nothing

                Console.Clear();
            }
        }

        static void SetWorkingDirectoryAndWorkName()
        {
            Console.WriteLine("Please enter the name of the class to grade for...");
            string wD = Console.ReadLine();

            Directory.SetCurrentDirectory(@"D:\Grading\" + wD);

            masterDirectory = Directory.GetCurrentDirectory().ToString();

            Console.WriteLine("Please enter the name of the file that is to be compiled");
            javaProgramName = Console.ReadLine();

            //string question = "Would you like to add an argument?";
            //if (GetBooleanResponse(question))
            //{
            //    Console.WriteLine("That is to fucking bad, the 201 students are not there yet.");
            //}
        }


        static void ReadAllMembersOfSection()
        {
            string inFile;

            sectionMembers = new List<string>();

            Console.WriteLine("Please enter the name of the file(also class section number, with class participants");
            inFile = Console.ReadLine();

            string ext = "txt";

            //inFile = inFile.Substring(0, inFile.IndexOf("."));

            //outFile = Directory.GetCurrentDirectory() + "\\" + fileName + "Out." + ext;

            inFile = Directory.GetCurrentDirectory() + "\\" + inFile + "." + ext;

            while (!File.Exists(inFile))
            {
                Console.WriteLine("File not found! Please re-enter the name of the file...");
                inFile = Console.ReadLine();
                inFile = Directory.GetCurrentDirectory() + "\\" + inFile + "." + ext;
            }

            StreamReader sR = new StreamReader(inFile);

            string line = sR.ReadLine();

            //StreamWriter sW = new StreamWriter(inFile + "New");
            while (line != null)
            {
                sectionMembers.Add(line);

                line = sR.ReadLine();
            }

            sR.Close();
        }

        static bool GetBooleanResponse(string questionToLayDown)
        {
            Console.WriteLine(questionToLayDown);
            string ans = Console.ReadLine();

            ans = ans.ToUpper();

            switch (ans)
            {
                case "Y":
                case "YES": return true;
                default: return false;

            }
        }

        static string ReturnCorrectedJavaName()
        {
            string [] files = Directory.GetFiles(Directory.GetCurrentDirectory().ToString());

            Console.WriteLine("We found the following files:\n");

            int cnt = files.Count();

            for (int i = 0; i < cnt; i++)
                files[i] = files[i].Remove(0, files[i].LastIndexOf("\\")+1);

            if (cnt <= 4)
            {
                foreach (string f in files)
                    Console.WriteLine(f);
            }
            else
            {
                
                for (int i = 0; i < cnt; i++)
                {
                    string output = files[i];

                    if (i % 2 == 0)
                        output += "\t";
                    else
                        output += "\n";

                    Console.Write(output);
                }
            }

            if (GetBooleanResponse("Do you see the file you are lookng for?"))
            {
                Console.WriteLine("Please type the name of the file");
                return Console.ReadLine();
            }
            else
            {
                string except = "File not found! Please check the following items:\n" + 
                    "\t1)Are you sure you have the right user selected...\n" + 
                    "\t2)Please check the spelling of the filename\n" + 
                    "Moving on...";
                Console.WriteLine(except);
                throw new NotAllThatBadException();
            }
        }

        static void CompileAndRunJavaProgram(string realName)
        {

            // Create process start info
            ProcessStartInfo psi = new ProcessStartInfo();

            /***********************************************************************************************/
            /*                                         COMPILE JAVA                                        */
            /***********************************************************************************************/
            // java File Name to compile
            psi.Arguments = realName + ".java";
            psi.RedirectStandardOutput = true;
            psi.FileName = "javac";
            psi.UseShellExecute = false;

            // Compile java project
            Process javac = Process.Start(psi);

            javac.WaitForExit();

            /***********************************************************************************************/
            /*                                           RUN JAVA                                          */
            /***********************************************************************************************/

            // run compiled project
            psi.Arguments = realName;
            psi.RedirectStandardOutput = false;
            psi.FileName = "java.exe";
            psi.CreateNoWindow = false;
            psi.ErrorDialog = true;
            psi.UseShellExecute = false;

            Process java = Process.Start(psi);

            while (!java.HasExited) { /*Do nothing*/ }

            java.WaitForExit();
        }
    }
}
