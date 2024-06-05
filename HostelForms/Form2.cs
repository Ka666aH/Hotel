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
using System.Data.Common;

/*todo

 */

namespace HostelForms
{
    public partial class Form2 : Form
    {
        public List<DateTime> bookingdates = new List<DateTime>();
        public string message = string.Empty;
        public decimal bill;

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
            if(comboBox2.Text == string.Empty)
            {
                message += "Выберите значение из списка в поле Комната";
            }
            if(comboBox3.Text == string.Empty)
            {
                message += "Выберите значение из списка в поле Комната";
            }
            if (message != string.Empty)
            {
                MessageBox.Show(message);
            }
            else
            {
                Guid guestguid;
                var HostelDb = new DbHostel();

                var guestquery1 = from guest in HostelDb.Guest
                                 select new Guest
                                 {
                                     GuestId = Guid.Empty,
                                     FullName = guest.FullName,
                                     BirthDate = guest.BirthDate,
                                     Gender = guest.Gender,
                                     Phone = guest.Phone
                                 };

                Guest guest1 = new Guest
                {
                    GuestId = Guid.Empty,
                    FullName = $"{textBox1.Text} {textBox2.Text} {textBox3.Text}",
                    BirthDate = dateTimePicker1.Value.Date,
                    Gender = checkBox1.Checked,
                    Phone = maskedTextBox1.Text
                };


                var bedquery = from bed in HostelDb.Bed
                               join room in HostelDb.Room on bed.RoomId equals room.RoomId      
                               where room.Number == short.Parse(comboBox2.Text) && bed.Number == short.Parse(comboBox3.Text)
                               select bed.BedId;

                var breakfastquery = from breakfast in HostelDb.Breakfast
                              where breakfast.Name == comboBox1.Text
                              select breakfast.BreakfastId;

                var statusquery = from status in HostelDb.Status
                                  where status.Name == "Ожидается оплата"
                                  select status.StatusId;
                string finalmessage;
                HostelDb.BeginTransaction();

                if (!guestquery1.Contains(guest1))
                {
                    guestguid = Guid.NewGuid();
                    HostelDb.Guest.Insert(() => new Guest
                    {
                        GuestId = guestguid,
                        FullName = $"{textBox1.Text} {textBox2.Text} {textBox3.Text}",
                        BirthDate = dateTimePicker1.Value.Date,
                        Gender = checkBox1.Checked,
                        Phone = maskedTextBox1.Text
                    });
                    finalmessage = "Бронирование успешно создано, новый гость создан";
                }
                else
                {
                    var guestquery2 = from guest in HostelDb.Guest
                                      select guest.GuestId;
                    guestguid = Guid.Parse(guestquery2.First().ToString());
                    finalmessage = "Бронирование успешно создано, гость найден";
                }

                Booking booking = new Booking();
                booking.BookingId = Guid.NewGuid();
                booking.BedId = Guid.Parse(bedquery.First().ToString());
                booking.GuestId = guestguid;
                booking.CheckInDate = dateTimePicker2.Value.Date;
                booking.CheckOutDate = dateTimePicker3.Value.Date;
                booking.BreakfastId = Guid.Parse(breakfastquery.First().ToString());
                booking.Bill = bill;
                booking.StatusId = Guid.Parse(statusquery.First().ToString());
                HostelDb.Insert(booking);

                foreach(DateTime date in bookingdates)
                {
                    BookedBed bookedbed = new BookedBed();
                    bookedbed.BookedBedId = Guid.NewGuid();
                    bookedbed.BedId = Guid.Parse(bedquery.First().ToString());
                    bookedbed.BookedDate = date.Date;
                    HostelDb.Insert(bookedbed);
                }

                HostelDb.CommitTransaction();
                MessageBox.Show(finalmessage);
                
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
                dateTimePicker1.Value = new DateTime(2000,1,1);
                checkBox1.Checked = true;
                dateTimePicker2.Value = DateTime.Today;
                dateTimePicker3.Value = DateTime.Today.AddDays(1);
                comboBox1.Text = comboBox1.Items[0].ToString();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                textBox4.Text = string.Empty;
                maskedTextBox1.Clear();
                //MessageBox.Show("yes");
                //HostelDb.RollbackTransaction();
                //Close();

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
            comboBox3.Items.Clear();
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
            dateTimePicker2.Value = DateTime.Today;
            dateTimePicker3.Value = DateTime.Today.AddDays(1);

            comboBox1.Items.Add("Нет");
            comboBox1.Text = comboBox1.Items[0].ToString();
            foreach (string name in GetBreakfastsName())
            {
                comboBox1.Items.Add(name);
            }

            //foreach(short number in GetAvalibleRoomsNumbers(checkBox1.Checked))
            //{
            //    comboBox2.Items.Add(number.ToString());
            //}

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

            var query = (from room in HostelDb.Room
                        join bed in HostelDb.Bed on room.RoomId equals bed.RoomId      
                        where !subquery1.Contains(bed.BedId) && room.TenantsGender == Gender
                        //orderby room.Number
                        select room.Number).Distinct();
            return query.OrderBy(a => a).ToList();
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
            if(dateTimePicker2.Value.Date<DateTime.Today)
            {
                dateTimePicker2.Value = DateTime.Today;
            }

            if(dateTimePicker2.Value.Date>=dateTimePicker3.Value.Date)
            {
                //MessageBox.Show("Даты пребывания гостя введены неверно");
                dateTimePicker3.Value = dateTimePicker2.Value.AddDays(1);
            }

            GetBookingDates(dateTimePicker2.Value, dateTimePicker3.Value);
            foreach (short number in GetAvalibleRoomsNumbers(checkBox1.Checked))
            {
                comboBox2.Items.Add(number.ToString());
            }
            comboBox3.Items.Clear();

            GetBill();

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            foreach(short number in GetBeds())
            {
                comboBox3.Items.Add(number);
            }
            GetBill();
        }

        public List<short> GetBeds()
        {
            using var HostelDb = new DbHostel();

            var subquery1 = from bookedbed in HostelDb.BookedBed
                            where bookingdates.Contains(bookedbed.BookedDate.Date)
                            select bookedbed.BedId;

            //MessageBox.Show(subquery1.First().ToString());

            var query = from bed in HostelDb.Bed
                        join room in HostelDb.Room on bed.RoomId equals room.RoomId
                        where room.Number == short.Parse(comboBox2.Text) && !subquery1.Contains(bed.BedId)
                        select bed.Number;
            return query.OrderBy(a => a).ToList();
        }

        public void GetBill()
        {
            using var HostelDb = new DbHostel();
            if(comboBox2.Text !=string.Empty && comboBox1.Text !=string.Empty)
            {

            var billquery1 = from room in HostelDb.Room
                             where room.Number == short.Parse(comboBox2.Text)
                             select room.PricePerBed;

            var billquery2 = from breakfast in HostelDb.Breakfast
                             where breakfast.Name == comboBox1.Text
                             select breakfast.Cost;

            decimal op1 = Convert.ToDecimal(billquery1.First().ToString());
            decimal op2 = Convert.ToDecimal(billquery2.First().ToString());
            bill = bookingdates.Count * (op1+op2);
            textBox4.Text = bill.ToString("C");
            //MessageBox.Show(decimal.Parse(textBox4.Text.Replace('₽',' ').Trim()).ToString());
            }
            else
            {
                textBox4.Text = string.Empty;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetBill();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
