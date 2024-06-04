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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            foreach(Booking booking in GetBookings())
            {
                //MessageBox.Show(booking.CheckInDate.ToString());
                using var HostelDb = new DbHostel();

                var query1 = from guest in HostelDb.Guest
                             where guest.GuestId == booking.GuestId
                             select guest.FullName;

                var query2 = from room in HostelDb.Room
                             //join bed in HostelDb.Bed on room.RoomId equals bed.RoomId
                             //where bed.BedId == booking.BedId
                             select room.Number;

                var query3 = from bed in HostelDb.Bed
                             where bed.BedId == booking.BedId
                             select bed.Number;

                var query4 = from breakfast in HostelDb.Breakfast
                             where breakfast.BreakfastId == booking.BreakfastId
                             select breakfast.Name;

                dataGridView1.Rows.Add(booking.BookingId, booking.GuestId, null, booking.BedId, booking.CheckInDate, booking.CheckOutDate, booking.BreakfastId, booking.Bill, booking.StatusId);
                //dataGridView1.Rows.Add(booking.BookingId,null,null,null,booking.CheckInDate,booking.CheckOutDate,null,booking.Bill,booking.StatusName);
            }
        }
        
        public List<Booking> GetBookings()
        {
            using var HosterDb = new DbHostel();
            var query = from bookings in HosterDb.Booking
                        select bookings;
            return query.ToList();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
