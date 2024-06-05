using HostelClasses;
using LinqToDB;
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
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            FillTable();
            using var HostelDb = new DbHostel();
            var methods = from m in HostelDb.Method
                          select m.Name;
            foreach (string name in methods)
            {
                comboBox1.Items.Add(name);
            }

            dateTimePicker1.Value = DateTime.Today;
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

                //dataGridView1.Rows.Add(booking.BookingId, booking.GuestId, null, booking.BedId, booking.CheckInDate, booking.CheckOutDate, booking.BreakfastId, booking.Bill, booking.StatusId);
                //dataGridView1.Rows.Add(booking.BookingId,null,null,null,booking.CheckInDate,booking.CheckOutDate,null,booking.Bill,booking.StatusName);
                dataGridView1.SelectionChanged -= new EventHandler(dataGridView1_SelectionChanged);
                dataGridView1.Rows.Add(booking.BookingId, query1.First().ToString(), query2.First().ToString(), query3.First().ToString()
                    , booking.CheckInDate, booking.CheckOutDate, query4.First().ToString(), booking.Bill.ToString("C"));
                dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
                
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView1.RowCount>0)
            dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = true;
        }

        public List<Booking> GetBookings()
        {
            using var HosterDb = new DbHostel();
            var query = from bookings in HosterDb.Booking
                        join status in HosterDb.Status on bookings.StatusId equals status.StatusId
                        where status.Name == "Ожидается оплата" 
                        select bookings;
            return query.ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Показать")
            {
                button1.Text = "Скрыть";
                dataGridView1.Visible = true;
                label4.Location = new Point(65, 341);
                dateTimePicker1.Location = new Point(189, 341);
                label5.Location = new Point(49, 370);
                comboBox1.Location = new Point(189, 370);
                button2.Location = new Point(189, 397);
                button3.Location = new Point(189, 426);
                button4.Location = new Point(189, 455);
                label3.Location = new Point(437, 10);
                Size = new Size(965, 530);

            }
            else 
            {
                button1.Text = "Показать";
                dataGridView1.Visible = false;
                label4.Location = new Point(65, 75);
                dateTimePicker1.Location = new Point(189, 75);
                label5.Location = new Point(49, 104);
                comboBox1.Location = new Point(189, 104);
                button2.Location = new Point(189, 131);
                button3.Location = new Point(189, 160);
                button4.Location = new Point(189, 189);
                label3.Location = new Point(135, 10);
                Size = new Size(360, 265);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using var HostelDb = new DbHostel();
            var query = from m in HostelDb.Method
                        where m.Name == comboBox1.Text
                        select m.MethodId;

            Payment payment = new Payment();
            payment.PaymentId = Guid.NewGuid();
            payment.BookingId = Guid.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            payment.Date = dateTimePicker1.Value.Date;
            payment.MethodId = Guid.Parse(query.First().ToString());

            var query1 = from b in HostelDb.Booking
                         where b.BookingId == Guid.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString())
                         select b;

            var query2 = from s in HostelDb.Status
                         where s.Name == "В процессе"
                         select s.StatusId;

            Booking booking = query1.First();
            booking.StatusId = Guid.Parse(query2.First().ToString());


            HostelDb.BeginTransaction();

            HostelDb.Insert(payment);
            HostelDb.Update(booking);

            HostelDb.CommitTransaction();

            MessageBox.Show("Платёж успешно зафиксирован, статус бронирования обновлён");
            FillTable();
            dateTimePicker1.Value = DateTime.Today;
            comboBox1.Text = string.Empty;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FillTable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close(); 

        }
    }
}
