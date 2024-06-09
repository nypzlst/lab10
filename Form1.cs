using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab10_1
{
    public partial class Form1 : Form
    {
        Thread thread1;
        Thread thread2;
        Thread thread3;

        public Form1()
        {
            InitializeComponent();
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button3.Click += new System.EventHandler(this.button3_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            thread1 = new Thread(new ThreadStart(RedocEncryption));
            thread1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            thread2 = new Thread(new ThreadStart(ShaHashing));
            thread2.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            thread3 = new Thread(new ThreadStart(LucEncryption));
            thread3.Start();
        }

        private void RedocEncryption()
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                   
                    aes.GenerateIV();
                    aes.Key = Encoding.UTF8.GetBytes("ThisIsAKey123456"); 
                    byte[] encrypted = PerformEncryption(aes, "Some data to aes");

                    richTextBox1.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.Text += $"REDOC Encrypted: {Convert.ToBase64String(encrypted)}\n";
                    });
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Text += $"REDOC Error: {ex.Message}\n";
                });
            }
        }


        private void ShaHashing()
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] data = Encoding.UTF8.GetBytes("Some data to hash");
                    byte[] hash = sha256.ComputeHash(data);

                    richTextBox1.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.Text += $"SHA Hash: {BitConverter.ToString(hash).Replace("-", "")}\n";
                    });
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Text += $"SHA Error: {ex.Message}\n";
                });
            }
        }

        private void LucEncryption()
        {
            try
            {
             
                BigInteger p = 139;
                BigInteger q = 149;
                BigInteger n = p * q;
                BigInteger e = 101;
                string plainText = "Some data to LUC!";

                BigInteger m = new BigInteger(Encoding.UTF8.GetBytes(plainText));

             
                BigInteger encrypted = LUCEncrypt(m, e, n);

                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Text += $"LUC Encrypted: {encrypted}\n";
                });
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Text += $"LUC Error: {ex.Message}\n";
                });
            }
        }

        private BigInteger LUCEncrypt(BigInteger m, BigInteger e, BigInteger n)
        {
           
            BigInteger Vk = LucasSequenceV(m, e, n);
            return Vk;
        }

        private BigInteger LucasSequenceV(BigInteger m, BigInteger e, BigInteger n)
        {
           
            BigInteger V0 = 2;
            BigInteger V1 = m;

            BigInteger V = V1;

            for (BigInteger i = 2; i <= e; i++)
            {
                V = (V1 * V0 - m) % n;
                V0 = V1;
                V1 = V;
            }

            return V;
        }


        private byte[] PerformEncryption(SymmetricAlgorithm algorithm, string plainText)
        {
            ICryptoTransform encryptor = algorithm.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread1?.Abort();
            thread2?.Abort();
            thread3?.Abort();
        }
    }
}
