﻿using HostelClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostelForms
{
    public partial class Form3 : Form
    {
        public static class Globals
        {
            public static short roomnumber;
            public static string guestname;
        }
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            foreach (short room in GetRooms())
            {
                listBox1.Items.Add(room);
            }
        }

        public List<short> GetRooms()
        {
            using var HostelDb = new DbHostel();
            var query = from room in HostelDb.Room orderby room.Number select room.Number;
            return query.ToList();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGuests();
        }
        public List<string> GetGuests()
        {
            List<string> list = new List<string>();
            using var HostelDb = new DbHostel();
            if(checkBox2.Checked)
            {
            var query = from guest in HostelDb.Guest
                        join booking in HostelDb.Booking on guest.GuestId equals booking.GuestId      
                        join bed in HostelDb.Bed on booking.BedId equals bed.BedId
                        join room in HostelDb.Room on bed.RoomId equals room.RoomId
                        join status in HostelDb.Status on booking.StatusId equals status.StatusId
                        where room.Number == Globals.roomnumber && status.Name =="Завершено"
                        select guest.FullName;
            list.AddRange(query.ToList());
            }
            if (checkBox3.Checked)
            {
                var query = from guest in HostelDb.Guest
                            join booking in HostelDb.Booking on guest.GuestId equals booking.GuestId
                            join bed in HostelDb.Bed on booking.BedId equals bed.BedId
                            join room in HostelDb.Room on bed.RoomId equals room.RoomId
                            join status in HostelDb.Status on booking.StatusId equals status.StatusId
                            where room.Number == Globals.roomnumber && status.Name == "В процессе"
                            select guest.FullName;
                list.AddRange(query.ToList());
            }
            if (checkBox4.Checked)
            {
                var query = from guest in HostelDb.Guest
                            join booking in HostelDb.Booking on guest.GuestId equals booking.GuestId
                            join bed in HostelDb.Bed on booking.BedId equals bed.BedId
                            join room in HostelDb.Room on bed.RoomId equals room.RoomId
                            join status in HostelDb.Status on booking.StatusId equals status.StatusId
                            where room.Number == Globals.roomnumber && status.Name == "Ожидается оплата"
                            select guest.FullName;
                list.AddRange(query.ToList());
            }

            return list;
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.guestname= listBox2.SelectedItem.ToString();
            dataGridView1.Rows.Clear();
            Guest guest =  GetGuestInfo();
            string guestgender;
            if(guest.Gender)
            {
                guestgender = "Мужской";
            }
            else 
            {
                guestgender = "Женский";
            }
            dataGridView1.Rows.Add(guest.FullName, guest.BirthDate.Date, guestgender ,guest.Phone);
        }
        public static Guest GetGuestInfo()
        {
            using var HostelDb = new DbHostel();
            var query = from guest in HostelDb.Guest
                        where guest.FullName == Globals.guestname
                        select guest;
            return query.First();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGuests();
        }
        
        public void UpdateGuests()
        {
            Globals.roomnumber = short.Parse(listBox1.SelectedItem.ToString());
            listBox2.Items.Clear();
            foreach (string guest in GetGuests())
            {
                listBox2.Items.Add(guest);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
