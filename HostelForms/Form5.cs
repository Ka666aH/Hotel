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
using HostelClasses;

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
            foreach(var x in GetRoom())
            {
                dataGridView1.Rows.Add(x.RoomId,x.Number, x.PricePerBed, x.TenantsGender,x.Description);
            }

            foreach (var x in GetBeds())
            {
                dataGridView6.Rows.Add(x.BedId,x.RoomId,x.Number);
            }

            foreach (var x in GetBookedBeds())
            {
                dataGridView9.Rows.Add(x.BookedBedId,x.BedId,x.BookedDate);
            }

            foreach (var x in GetGuests())
            {
                dataGridView2.Rows.Add(x.GuestId,x.FullName,x.BirthDate,x.Gender,x.Phone);
            }

            foreach (var x in GetBreaks())
            {
                dataGridView5.Rows.Add(x.BreakfastId,x.Name,x.Cost);
            }

            foreach (var x in GetStatuses())
            {
                dataGridView8.Rows.Add(x.StatusId,x.Name);
            }

            foreach (var x in GetBooking())
            {
                dataGridView3.Rows.Add(x.BookingId,x.BedId,x.GuestId,x.CheckInDate,x.CheckOutDate,x.BreakfastId,x.Bill,x.StatusId);
            }

            foreach (var x in GetMethods())
            {
                dataGridView4.Rows.Add(x.MethodId,x.Name);
            }

            foreach (var x in GetPayments())
            {
                dataGridView7.Rows.Add(x.PaymentId,x.BookingId,x.Date,x.MethodId);
            }
        }

        public List<Bed> GetBeds()
        {
            using var H = new DbHostel();
            var q = from b in H.Bed
                    select b;
            return q.ToList();
        }
        public List<Room> GetRoom()
        {
            using var H = new DbHostel();
            var q = from b in H.Room
                    select b;
            return q.ToList();
        }
        public List<BookedBed> GetBookedBeds()
        {
            using var H = new DbHostel();
            var q = from b in H.BookedBed
                    select b;
            return q.ToList();
        }
        public List<Guest> GetGuests()
        {
            using var H = new DbHostel();
            var q = from b in H.Guest
                    select b;
            return q.ToList();
        }
        public List<Breakfast> GetBreaks()
        {
            using var H = new DbHostel();
            var q = from b in H.Breakfast
                    select b;
            return q.ToList();
        }
        public List<Status> GetStatuses()
        {
            using var H = new DbHostel();
            var q = from b in H.Status
                    select b;
            return q.ToList();
        }
        public List<Booking> GetBooking()
        {
            using var H = new DbHostel();
            var q = from b in H.Booking
                    select b;
            return q.ToList();
        }
        public List<Method> GetMethods()
        {
            using var H = new DbHostel();
            var q = from b in H.Method
                    select b;
            return q.ToList();
        }
        public List<Payment> GetPayments()
        {
            using var H = new DbHostel();
            var q = from b in H.Payment
                    select b;
            return q.ToList();
        }
    }
}
