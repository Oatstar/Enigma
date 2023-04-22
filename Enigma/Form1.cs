using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Enigma
{
    public partial class Form1 : Form
    {
        public string inputStringCleartext;
        public string outputStringEncrypted;

        public string inputStringEncrypted;
        public string outputStringCleartext;

        public int rotorLength = 26;
        public int[] rotorSettings = new int[] {0,0,0 };


        //Currently all the rotors are the same. The functionalities still work, but "randomization" isn't as good as it can be
        int[,] rotorConnectionsDefault = new int[3, 26]
            {
                {1, 3, 5, 7, 9, 11, 2, 15, 17, 19, 23, 21, 25, 13, 24, 4, 8, 22, 6, 0, 10, 12, 20, 18, 16, 14  },
                {1, 3, 5, 7, 9, 11, 2, 15, 17, 19, 23, 21, 25, 13, 24, 4, 8, 22, 6, 0, 10, 12, 20, 18, 16, 14  },
                {1, 3, 5, 7, 9, 11, 2, 15, 17, 19, 23, 21, 25, 13, 24, 4, 8, 22, 6, 0, 10, 12, 20, 18, 16, 14  }
            };

        int[,] rotorConnections = new int[3, 26]
            {
                {1, 3, 5, 7, 9, 11, 2, 15, 17, 19, 23, 21, 25, 13, 24, 4, 8, 22, 6, 0, 10, 12, 20, 18, 16, 14  },
                {1, 3, 5, 7, 9, 11, 2, 15, 17, 19, 23, 21, 25, 13, 24, 4, 8, 22, 6, 0, 10, 12, 20, 18, 16, 14  },
                {1, 3, 5, 7, 9, 11, 2, 15, 17, 19, 23, 21, 25, 13, 24, 4, 8, 22, 6, 0, 10, 12, 20, 18, 16, 14  }
            };


        int[] reflector = new int[26]
            {22, 19, 14, 6, 10, 25, 3, 13, 11, 20, 4, 8, 15, 7, 2, 12, 18, 23, 16, 1, 9, 24, 0, 17, 21, 5};


       /*      Key goes in, goes through 3 rotors and gets reflected through the reflector, 
        *      passing once again each of the 3 rotors.
        *      |-<---------<------------<---------<--------<Input
        *      |->------->------------>--------->---------->Output
        *      |Reflector| Rotor2  | Rotor1  | Rotor0  | Keypad         
        *      | 0->22   | 1       | 1       | 1       | A         
        *      | 1->19   | 3       | 3       | 3       | B       
        *      | 2->14   | 5       | 5       | 5       | C        
        *      | 3->6    | 7       | 7       | 7       | D        
        *      | 4->10   | 9       | 9       | 9       | E        
        *      | 5->25   | 11      | 11      | 11      | F        
        *      | ...     | ...     | ...     | ...     | ...        
        */


        // 0    1    2    3    4    5    6    7    8    9   10   11   12   13   14   15   16   17   18   19   20   21   22   23   24   25
        char[] alphabets = new char[26] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        
        public Form1()
        {
            InitializeComponent();
        }

        void DoEncrypting()
        {
            //TESTING THE FUNCTION
            string debugText = "";
            for (int i = 0; i < rotorLength; i++)
            {
                debugText += " " + rotorConnections[0, i];
            }

            Debug.WriteLine("------");
            Debug.WriteLine("Rotor0Connections: " + debugText);

            foreach (char c in inputStringCleartext)
            {
                if (c == ' ')
                    outputStringEncrypted += c;

                for (int i = 0; i < alphabets.Length; i++)
                {
                    if (c == alphabets[i])
                    {
                        int rotor0Index = FeedValueIntoRotors(i);
                        outputStringEncrypted += alphabets[rotor0Index];
                        
                        Debug.WriteLine(c + " = " + alphabets[rotorConnections[1, i]]);
                        SpinRotor(0);
                    }
                 }
            }
            label3.Text = outputStringEncrypted;
        }

        void DoDecrypting()
        {
            outputStringCleartext = "";

            foreach (char c in inputStringEncrypted)
            {
                if (c == ' ')
                    outputStringCleartext += c;

                for (int i = 0; i < alphabets.Length; i++)
                {
                    if (c == alphabets[i])
                    {
                        int rotor0Index = FeedValueIntoRotors(i);

                        outputStringCleartext += alphabets[rotor0Index];
                        Debug.WriteLine("outputStringClearText: " + outputStringCleartext);
                        SpinRotor(0);
                    }

                }
            }
            label11.Text = outputStringCleartext;
        }

        int FeedValueIntoRotors(int firstValue)
        {
            int rotor0Index = 0;
            int rotor1Index = 0;
            int rotor2Index = 0;

            int rotor0Value = rotorConnections[0, firstValue];
            int rotor1Value = rotorConnections[1, rotor0Value];
            int rotor2Value = rotorConnections[2, rotor1Value];

            int reflectorValue = reflector[rotor2Value];

            for (int j = 0; j < rotorLength; j++)
            {
                if (rotorConnections[2, j] == reflectorValue)
                    rotor2Index = j;
            }

            for (int j = 0; j < rotorLength; j++)
            {
                if (rotorConnections[1, j] == rotor2Index)
                    rotor1Index = j;
            }

            for (int j = 0; j < rotorLength; j++)
            {
                if (rotorConnections[0, j] == rotor1Index)
                    rotor0Index = j;
            }

            Debug.WriteLine("Rotor0 value is: " + rotor0Value);
            Debug.WriteLine("Rotor1 value is: " + rotor1Value);
            Debug.WriteLine("Rotor2 value is: " + rotor2Value);

            return rotor0Index;
        }

        void SpinRotor(int rotorNumber)
        {
            int[,] tempRotorConnections = new int[3,rotorLength];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    tempRotorConnections[i, j] = rotorConnections[i, j];
                }
            }

            for (int i = 0; i < rotorLength; i++)
            {
                if (i == 25)
                    rotorConnections[rotorNumber, i] = tempRotorConnections[rotorNumber, 0];
                else
                    rotorConnections[rotorNumber, i] = tempRotorConnections[rotorNumber, i + 1];
            }

            if (rotorConnections[rotorNumber,0] == rotorConnectionsDefault[rotorNumber,0])
            {
                if(rotorNumber == 3)
                    SpinRotor(1);
                else
                    SpinRotor(rotorNumber+1);
            }
                
            //TESTING THE FUNCTION
            string debugText = "";
            for (int i = 0; i < rotorLength; i++)
            {
                debugText += " " + rotorConnections[0, i];
            }

            Debug.WriteLine("------");
            Debug.WriteLine("Rotor0Connections: "+ debugText);
        }
      
        void ResetRotors()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    rotorConnections[i, j] = rotorConnectionsDefault[i, j];
                }
            }
            textBoxRotor1.Text = "";
            textBoxRotor2.Text = "";
            textBoxRotor3.Text = "";

            rotorSettingsLabel.Text = "";
        }

        void SetRotors()
        {
            //For each rotor (3 times)
            for (int currentRotor = 0; currentRotor < 3; currentRotor++)
            {

                //Copy the default rotors to temporary array
                int[,] tempRotorConnections = new int[3, rotorLength];

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 26; j++)
                    {
                        tempRotorConnections[i, j] = rotorConnectionsDefault[i, j];
                    }
                }

                //For each index in rotor copy the defaults as new rotorConnections values
                for (int i = 0; i < rotorLength; i++)
                {
                    if (i + rotorSettings[currentRotor] < 26)
                        rotorConnections[currentRotor, i] = tempRotorConnections[currentRotor, i+rotorSettings[currentRotor]];
                    else
                        rotorConnections[currentRotor, i] = tempRotorConnections[currentRotor, i + rotorSettings[currentRotor] - rotorLength];
                }

                //TESTING THE FUNCTION
                string debugText = "";
                for (int i = 0; i < rotorLength; i++)
                {
                    debugText += " " + rotorConnections[currentRotor, i];
                }

                Debug.WriteLine("------");
                Debug.WriteLine(debugText);
            }
        }

        //Do encrypting
        private void button2_Click_1(object sender, EventArgs e)
        {
            //Start Encrypting
            outputStringEncrypted = "";
            inputStringCleartext = textBox1.Text;
            DoEncrypting();
        }

        //Debug stuff
        private void button3_Click(object sender, EventArgs e)
        {
            //SpinRotor(1);
        }

        //Reset rotors
        private void button4_Click(object sender, EventArgs e)
        {
            ResetRotors();
        }


        //Save rotor settings
        private void button5_Click(object sender, EventArgs e)
        {
            rotorSettings[0] = int.Parse(textBoxRotor1.Text);
            rotorSettings[1] = int.Parse(textBoxRotor2.Text);
            rotorSettings[2] = int.Parse(textBoxRotor3.Text);
            Debug.WriteLine("ASD " + textBoxRotor1.Text + ", " + textBoxRotor2.Text + ", " + textBoxRotor3.Text);
            rotorSettingsLabel.Text = textBoxRotor1.Text + ", " + textBoxRotor2.Text + ", " + textBoxRotor3.Text;

            SetRotors();
        }


        //Start Decrypting given text
        private void button6_Click(object sender, EventArgs e)
        {
            inputStringEncrypted = textBox2.Text;
            label11.Text = "";
            DoDecrypting();
        }


    }
}
