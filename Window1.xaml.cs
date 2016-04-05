using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;



namespace Backup_Update
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool error=true;
        String ftpPath = "ftp://127.0.0.1/";
        public Window1()
        {
            InitializeComponent();
            loginregisterbtngrid.Visibility = Visibility.Visible;
            loginGrid.Visibility = Visibility.Hidden;
            registrationGrid.Visibility = Visibility.Hidden;
            fgptext.Visibility = Visibility.Hidden;
            fgptextblock.Visibility = Visibility.Hidden;
            secAnswerText.Visibility = Visibility.Hidden;
            
        }

       //titlebar controls
            private void titlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

            private void close_Click(object sender, RoutedEventArgs e)
            {
                Window1 w = new Window1();

                w.Close();
            }

            private void close_Click_1(object sender, RoutedEventArgs e)
            {
                this.Close();
            }

            public void min_MouseDown(object sender, MouseButtonEventArgs e)
            {
                this.WindowState = System.Windows.WindowState.Minimized;


            }
        //==================================================================================

        //Entry buttons
            private void loginentrybtn_Click(object sender, RoutedEventArgs e)
            {
                loginregisterbtngrid.Visibility = Visibility.Hidden;
                loginGrid.Visibility = Visibility.Visible;
                LoginButton.IsDefault = true;
                registrationButton.IsDefault = false;
            }

            private void registerentrybtn_Click(object sender, RoutedEventArgs e)
            {
                loginregisterbtngrid.Visibility = Visibility.Hidden;
                registrationGrid.Visibility = Visibility.Visible;
                registrationButton.IsDefault = true;
                LoginButton.IsDefault = false;
            }
        //==================================================================================


                                   //   Login window   //
            private void LoginButton_Click(object sender, RoutedEventArgs e)
            {

                SqlConnection sc = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\mihir\Desktop\Backup_Update-2014-03-01\Backup_Update\Backup_Update\Database1.mdf;Integrated Security=True");
                    sc.Open();
                    String login = "select * from [dbo].[Login] where username='" + username.Text + "' and password='" + password.Password + "'";
                    SqlCommand cmd = new SqlCommand(login, sc);
                    if (cmd.ExecuteScalar() != null)
                    {

                        MainWindow mw = new MainWindow(username.Text);
                        this.Hide();
                        mw.Show();

                    }
                    else
                    {
                        MessageBox.Show("Wrong Username or Password");
                        username.Text = "";
                        password.Password = "";
                    }
               
            }

            private void fgp_Click(object sender, RoutedEventArgs e)
            {
                fgptext.Visibility = Visibility.Visible;
                fgptextblock.Visibility = Visibility.Visible;

            }

            private void fgptext_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    if (fgptext.Text != "")
                    {
                        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\mihir\Desktop\Backup_Update-2014-03-01\Backup_Update\Backup_Update\Database1.mdf;Integrated Security=True");
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SELECT security_question FROM Login WHERE username='" + fgptext.Text + "'", con);
                        cmd.ExecuteNonQuery();
                        if (cmd.ExecuteScalar() != null)
                        {
                            fgptextblock.Text = cmd.ExecuteScalar().ToString();
                            secAnswerText.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageBox.Show("Invalid username");
                            fgptext.Text = "";
                        }
                    }
                }
            }

            private void secAnswerText_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Enter)
                {
                    SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\mihir\Desktop\Backup_Update-2014-03-01\Backup_Update\Backup_Update\Database1.mdf;Integrated Security=True");
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT password FROM Login WHERE username='" + fgptext.Text + "' AND security_answer= '" + secAnswerText.Text.Trim() + "'", con);
                    cmd.ExecuteNonQuery();
                    if (cmd.ExecuteScalar() != null)
                    {
                        securityPasswoedBlock.Text = cmd.ExecuteScalar().ToString();
                    }
                    else
                    {
                        MessageBox.Show("wrong answer");
                        secAnswerText.Text = "";
                        securityPasswoedBlock.Text = "";
                    }

                }
            }

            void fgptext_GotFocus(object sender, RoutedEventArgs e)
            {
                fgptextblock.Text = "";
                securityPasswoedBlock.Text = "";
                secAnswerText.Text = "";

                secAnswerText.Visibility = Visibility.Hidden;
                if (fgptext.Text == "Enter username")
                    fgptext.Text = "";

            }

            private void secAnswerText_GotFocus(object sender, RoutedEventArgs e)
            {
                if (secAnswerText.Text == "Answer")
                    secAnswerText.Text = "";
            }

            private void backlogin_Click(object sender, RoutedEventArgs e)
            {
                loginregisterbtngrid.Visibility = Visibility.Visible;
                loginGrid.Visibility = Visibility.Hidden;
            }
        //=====================================================================================
           
            
                                 //   Registration window    //
        
        
        // Validations
            private void regusernametext_LostFocus(object sender, RoutedEventArgs e)
            {
                SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\mihir\Desktop\Backup_Update-2014-03-01\Backup_Update\Backup_Update\Database1.mdf;Integrated Security=True");
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Login where username = @UserID", con);
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@UserID";
                param.Value = regusernametext.Text;
                cmd.Parameters.Add(param);
                if (regusernametext.Text.Length < 4 || regusernametext.Text=="")
                {
                    valusername.Text = "The username must be between 4 and 10 characters long.";
                    error = true;
                }
               
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                    MessageBox.Show("User name already exists..Please try a new one");
                    regusernametext.Text = "";
                    error = true;
                }
            }

            private void regusernametext_GotFocus(object sender, RoutedEventArgs e)
            {
                valusername.Text = "";
                error = false;
            }

            private void regpasswordtext_GotFocus(object sender, RoutedEventArgs e)
            {
                valpassword.Text = "";
                
            }

            private void regpasswordtext_LostFocus(object sender, RoutedEventArgs e)
            {
                if (regpasswordtext.Password == "")
                {
                    valpassword.Text = "Enter the password";
                    error = true;
                }
                else if (regpasswordtext.Password.Length <= 6)
                {
                    valpassword.Text = "password must be greater than 6 characters";
                    error = true;
                }
            }

            private void regconpasswordtext_LostFocus_1(object sender, RoutedEventArgs e)
            {
                if (regpasswordtext.Password != regconpasswordtext.Password)
                {
                    valconfpassword.Text = "password does not match";
                    error = true;
                }
                else
                    valconfpassword.Text = "";
            }

            private void regconpasswordtext_GotFocus(object sender, RoutedEventArgs e)
            {
                regconpasswordtext.Password = "";
                valconfpassword.Text = "";
            }

            private void regsecuritytext_LostFocus(object sender, RoutedEventArgs e)
            {
                if (regsecuritytext.Text == "")
                {
                    valSecurity.Text = "Enter answer";
                    error = true;
                }
            }

            private void regsecuritytext_GotFocus(object sender, RoutedEventArgs e)
            {
                valSecurity.Text = "";
                error = false;
            }


            private void backregister_Click(object sender, RoutedEventArgs e)
            {
                registrationGrid.Visibility = Visibility.Hidden;
                loginregisterbtngrid.Visibility = Visibility.Visible;
            }

            private void regrefreshbtn_Click(object sender, RoutedEventArgs e)
            {
                regusernametext.Text = "";
                regpasswordtext.Password = "";
                regconpasswordtext.Password = "";
                regsecuritytext.Text = "";
                regsecuritylist.SelectedIndex = 0;
                valconfpassword.Text = "";
                valpassword.Text="";
                valusername.Text="";
                valSecurity.Text="";
            }


            //Register button
            private void registrationButton_Click_1(object sender, RoutedEventArgs e)
            {
                bool empty = false;
                if (regusernametext.Text == "" || regpasswordtext.Password == "" || regconpasswordtext.Password == "" || regsecuritytext.Text == "")
                {
                    empty = true;
                }
                if (!error && !empty )
                {
                    SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\mihir\Desktop\Backup_Update-2014-03-01\Backup_Update\Backup_Update\Database1.mdf;Integrated Security=True");
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Login (username,password,confirm_password,security_question,security_answer) VALUES ('" + regusernametext.Text + "','" + regpasswordtext.Password + "','" + regconpasswordtext.Password + "','" + regsecuritylist.Text + "','" + regsecuritytext.Text + "')", con);
                    cmd.ExecuteNonQuery();


                    con.Close();
                    MessageBox.Show("THANK YOU FOR REGISTRATION");
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath + regusernametext.Text);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    
                    WebResponse response = request.GetResponse();
                    DirectoryInfo di = Directory.CreateDirectory("C:\\backup\\" + regusernametext.Text);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
                else
                {
                    valusername.Text = "Enter your username";
                    valpassword.Text = "Enter the password";
                    valconfpassword.Text = "Please Confirm the above password";
                    valSecurity.Text = "Select security question and write appropriate answer";
                    MessageBox.Show("Please correct the errors");
                }


            }



            
        
    }
   
}
