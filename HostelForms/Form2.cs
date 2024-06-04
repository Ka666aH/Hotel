using HostelClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LinqToDB;

namespace HostelForms
{
    public partial class Form2 : Form
    {
        public List<DateTime> bookingdates = new List<DateTime>();
        public string message = string.Empty;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            message = string.Empty;

            if (textBox1.Text == string.Empty)
            {
                message += "Заполните поле Фамилия\n";
            }
            if (textBox2.Text == string.Empty)
            {
                message += "Заполните поле Имя\n";
            }
            if (textBox3.Text == string.Empty)
            {
                message += "Заполните поле Отчество\n";
            }
            if (maskedTextBox1.Text.Length < 12)
            {
                message += "Заполните поле Номер телефона";
            }
            if (message != string.Empty)
            {
                MessageBox.Show(message);
            }
            else
            {
                var HostelDb = new DbHostel();
                Guid newguestguid = Guid.NewGuid();
                HostelDb.Guest.Insert(() => new Guest
                {
                    GuestId = newguestguid,
                    FullName = $"{textBox1.Text} {textBox2.Text} {textBox3.Text}",
                    BirthDate = dateTimePicker1.Value.Date,
                    Gender = checkBox1.Checked,
                    Phone = maskedTextBox1.Text
                });

                var bedquery = from bed in HostelDb.Bed
                             //join room in HostelDb.Room on bed.RoomId equals room.RoomId      //Не работает
                             //where room.Number == short.Parse(comboBox2.Text)
                             where bed.Number == short.Parse(comboBox3.Text)
                             select bed.BedId;

                var breakfastquery = from breakfast in HostelDb.Breakfast
                              where breakfast.Name == comboBox1.Text
                              select breakfast.BreakfastId;

                var billquery1 = from room in HostelDb.Room
                                where room.Number == short.Parse(comboBox2.Text)
                                select room.PricePerBed;

                var billquery2 = from breakfast in HostelDb.Breakfast
                                 where breakfast.Name == comboBox1.Text
                                 select breakfast.Cost;

                decimal bill = bookingdates.Count * (Convert.ToDecimal(billquery1) + Convert.ToDecimal(billquery2));

                HostelDb.Booking.Insert(() => new Booking
                {
                    BookingId = Guid.NewGuid(),
                    BedId = Guid.Parse(bedquery.ToString()),
                    GuestId = newguestguid,
                    CheckInDate = dateTimePicker2.Value.Date,
                    CheckOutDate = dateTimePicker3.Value.Date,
                    BreakfastId = Guid.Parse(breakfastquery.ToString()),
                    Bill = bill,
                    StatusName = "Ожидается оплата"
                    
                });
                Close();

            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                
            }
            else
            {
                checkBox2.Checked = true;
            }

            foreach (short number in GetAvalibleRoomsNumbers(checkBox1.Checked))
            {
                comboBox2.Items.Add(number.ToString());
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }

            foreach (short number in GetAvalibleRoomsNumbers(checkBox1.Checked))
            {
                comboBox2.Items.Add(number.ToString());
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker3.Value = DateTime.Now.AddDays(1);

            comboBox1.Items.Add("Нет");
            comboBox1.Text = comboBox1.Items[0].ToString();
            foreach (string name in GetBreakfastsName())
            {
                comboBox1.Items.Add(name);
            }

            foreach(short number in GetAvalibleRoomsNumbers(checkBox1.Checked))
            {
                comboBox2.Items.Add(number.ToString());
            }

        }

        public List<string> GetBreakfastsName()
        {
            using var HostelDb = new DbHostel();
            var query = from breakfast in HostelDb.Breakfast
                        where breakfast.Name != "Нет"
                        orderby breakfast.Name 
                        select breakfast.Name;
            return query.ToList();
        }
        public List<short> GetAvalibleRoomsNumbers(bool Gender)
        {
         
            comboBox2.Items.Clear();
            using var HostelDb = new DbHostel();

            var subquery1 = from bookedbed in HostelDb.BookedBed
                            where bookingdates.Contains(bookedbed.BookedDate)
                            select bookedbed.BedId;

            var query = from room in HostelDb.Room
                        //.LoadWith(ri => ri.RoomId)
                        //    .ThenLoad(b => b.)

                        //join bed in HostelDb.Bed on room.RoomId equals bed.RoomId     //Не работает
                        //where !subquery1.Contains(bed.BedId)
                        where room.TenantsGender == Gender
                        orderby room.Number
                        select room.Number;
            return query/*.LoadWith(q => q.Bed)*/.ToList();
        }

        public void GetBookingDates(DateTime startdate, DateTime enddate)
        {
            bookingdates.Clear();
            for (int i = 0; i < (enddate-startdate).TotalDays;i++)
            {
                bookingdates.Add(startdate.AddDays(i));
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if(dateTimePicker2.Value>=dateTimePicker3.Value)
            {
                //MessageBox.Show("Даты пребывания гостя введены неверно");
                dateTimePicker3.Value = dateTimePicker2.Value.AddDays(1);
            }
            GetBookingDates(dateTimePicker2.Value, dateTimePicker3.Value);
        }
    }
}
