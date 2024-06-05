using HostelClasses;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostelForms
{
    public partial class Form4 : Form
    {
        public List<DateTime> bookingdates = new List<DateTime>();
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            FillTable();
        }
        
        public List<Booking> GetBookings()
        {
            using var HosterDb = new DbHostel();
            var query = from bookings in HosterDb.Booking
                        join status in HosterDb.Status on bookings.StatusId equals status.StatusId
                        where status.Name != "Завершено" && status.Name != "Отменено"
                        select bookings;
            return query.ToList();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            string cellValue = dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString();
            //MessageBox.Show(cellValue);
            comboBox1.Items.Clear();
            comboBox1.Items.Add(cellValue);
            if(cellValue == "Ожидается оплата")
            {
                comboBox1.Items.Add("Отменено");
            }
            else 
            {
                if(cellValue == "В процессе")
                comboBox1.Items.Add("Завершено");
            }
            comboBox1.Text = cellValue;

        }
        public void FillTable()
        {
            dataGridView1.Rows.Clear();
            using var HostelDb = new DbHostel();

            foreach (Booking booking in GetBookings())
            {
                //MessageBox.Show(booking.CheckInDate.ToString());

                var query1 = from guest in HostelDb.Guest
                             where guest.GuestId == booking.GuestId
                             select guest.FullName;

                var query2 = from room in HostelDb.Room
                             join bed in HostelDb.Bed on room.RoomId equals bed.RoomId
                             where bed.BedId == booking.BedId
                             select room.Number;

                var query3 = from bed in HostelDb.Bed
                             where bed.BedId == booking.BedId
                             select bed.Number;

                var query4 = from breakfast in HostelDb.Breakfast
                             where breakfast.BreakfastId == booking.BreakfastId
                             select breakfast.Name;

                var query5 = from status in HostelDb.Status
                             where status.StatusId == booking.StatusId
                             select status.Name;

                //dataGridView1.Rows.Add(booking.BookingId, booking.GuestId, null, booking.BedId, booking.CheckInDate, booking.CheckOutDate, booking.BreakfastId, booking.Bill, booking.StatusId);
                //dataGridView1.Rows.Add(booking.BookingId,null,null,null,booking.CheckInDate,booking.CheckOutDate,null,booking.Bill,booking.StatusName);
                dataGridView1.SelectionChanged -= new EventHandler(dataGridView1_SelectionChanged);
                dataGridView1.Rows.Add(booking.BookingId, query1.First().ToString(), query2.First().ToString(), query3.First().ToString()
                    , booking.CheckInDate, booking.CheckOutDate, query4.First().ToString(), booking.Bill.ToString("C"), query5.First().ToString());
                dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
                //dataGridView1.CurrentCell = dataGridView1[dataGridView1.CurrentRow.Cells.Count - 1, dataGridView1.CurrentRow.Index];
                dataGridView1.CurrentCell = dataGridView1[8, dataGridView1.CurrentRow.Index];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FillTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1[8, dataGridView1.CurrentRow.Index].Value.ToString() != comboBox1.Text)
            {
                dataGridView1[8, dataGridView1.CurrentRow.Index].Value = comboBox1.Text;

                using var HostelDb = new DbHostel();

                var query1 = from b in HostelDb.Booking
                             where b.BookingId == Guid.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString())
                             select b;

                var query2 = from status in HostelDb.Status
                             where status.Name == comboBox1.Text
                             select status.StatusId;

                Booking booking = query1.First();
                booking.StatusId = Guid.Parse(query2.First().ToString());
                //MessageBox.Show(booking.StatusId.ToString());




                HostelDb.BeginTransaction();

                HostelDb.Update(booking);

                if (comboBox1.Text == "Отменено")
                {
                    for (int i = 0; i < (booking.CheckOutDate.Date - booking.CheckInDate.Date).TotalDays; i++)
                    {
                        bookingdates.Add(booking.CheckInDate.Date.AddDays(i));
                    }

                    var query3 = from bookings in HostelDb.Booking
                                 join beds in HostelDb.Bed on bookings.BedId equals beds.BedId
                                 join bookedbeds in HostelDb.BookedBed on beds.BedId equals bookedbeds.BedId
                                 where bookingdates.Contains(bookedbeds.BookedDate)
                                 select bookedbeds.BookedBedId;

                    HostelDb.BookedBed.Where(bb => query3.Contains(bb.BookedBedId)).Delete();

                }

                HostelDb.CommitTransaction();
                MessageBox.Show("Cтатус бронирования успешно обновлён");
                FillTable();

            }
        }
    }
}
