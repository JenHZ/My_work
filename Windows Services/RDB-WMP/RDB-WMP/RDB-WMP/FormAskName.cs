/*
*Filename:		FormAskName.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormAskName, this form is used to ask for user name and ip address
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RDB_WMP
{
    /// NAME	:	FormAskName
    /// PURPOSE :   This form is used to get name and IP address from a user.
    public partial class FormAskName : Form
    {
        private string myName;              // used to get user name
        private IPAddress ipAddress;        // used to hold the ip address
        private const int PORT = 5000;      // port used for socket
        private IPEndPoint ipep = null;     // ip end point
        private Socket server = null;       // used to communicate with server



        ///Function:		FormAskName constructor
        ///Description:     called to initiate an instance
        ///Parameters:      NONE
        ///Return Values:   NONE
        public FormAskName()
        {
            InitializeComponent();
            myName = null;
            ipAddress = null;
        }




        ///property of ipep
        public IPEndPoint Ipep
        {
            get
            {
                return ipep;
            }
        }

        ///property of myName
        public string MyName
        {
            get
            {
                return myName;
            }
        }

        ///property of ipAddress
        public IPAddress IpAddress
        {
            get
            {
                return ipAddress;
            }
        }

        ///property of server
        public Socket Server
        {
            get
            {
                return server;
            }
        }

        



        ///Function:		btnName_Click
        ///Description:     after user enter the name and ip address, the method will try to connect to the server
        ///Parameters:      object sender:   the sender of the event
        ///                 EventArgs e:     event argument
        ///Return Values:   NONE
        private void btnName_Click(object sender, EventArgs e)
        {
            // user name can't be empty
            if (txtBoxName.Text == "")
            {
                MessageBox.Show("Name cannot be empty");
            }
            // user name can't be empty
            else if (txtBoxIP.Text == "")
            {
                MessageBox.Show("IP address cannot be empty");
            }
            else
            {
                try
                {
                    // try to get the ip address in the textbox, if the format is not correct, will tell the user
                    ipAddress = IPAddress.Parse(this.txtBoxIP.Text);
                    myName = txtBoxName.Text;
                    ipep = new IPEndPoint(ipAddress, PORT);
                }
                // output error message if there's something wrong with user input
                catch (ArgumentNullException)
                {
                    this.lblError.Text = "IP address is not correct";
                }
                catch (FormatException)
                {
                    this.lblError.Text = "Format of IP address is not correct";
                }
                catch (Exception ex)
                {
                    this.lblError.Text = ex.ToString();
                }

                if (ipep != null)
                {
                    this.lblError.Text = "Connecting to the Server...";
                }


                // create the socket, using tcp
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // try to connect to the server
                try
                {
                    server.Connect(ipep);
                    this.lblError.Text = "Connected to the Server.";
                    this.Close();
                }
                catch (SocketException exectpion)
                {
                    this.lblError.Text = "Can not connect to Server: " + ipep.Address
                        + "\n" + "SocketErrorCode: " + exectpion.SocketErrorCode;
                }
            }
        }
    }
}
