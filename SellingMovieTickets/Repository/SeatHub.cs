using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;

namespace SellingMovieTickets.Repository
{
    public class SeatHub : Hub
    {
        private readonly DataContext _context;

        public SeatHub(DataContext context)
        {
            _context = context;
        }

        public async Task UpdateSeatStatus(int seatId, string status, string test, string userHeldByUserId)
        {
            var seat = new SeatModel();
            var statusReturn = "";
            if (seatId != null)
            {
                seat = await _context.Seats.FindAsync(seatId);
            }
            try
            {
                if (status == "Payment")
                {
                    var listSeat = await _context.Seats.Where(x => x.HeldByUserId == userHeldByUserId).ToListAsync();
                    foreach (var item in listSeat)
                    {
                        item.IsHeld = false;
                        item.HoldUntil = DateTime.Now;
                        item.HeldByUserId = null;
                    }
                    await _context.SaveChangesAsync();
                    await Clients.All.SendAsync("ListSeatStatusUpdated", "Payment", listSeat);
                }
                else if (status != "Payment")
                {
                    if (status == "Held")
                    {
                        seat.IsHeld = true;
                        seat.HoldUntil = DateTime.UtcNow.AddMinutes(3);
                        seat.HeldByUserId = userHeldByUserId;
                        statusReturn = "Held";
                    }
                    else if (status == "CancelHeld")
                    {
                        seat.IsHeld = false;
                        seat.HoldUntil = DateTime.Now;
                        seat.HeldByUserId = null;
                        statusReturn = "CancelHeld";
                    }

                    var seatData = new
                    {
                        SeatId = seat.Id,
                        SeatNumber = seat.SeatNumber,
                        IsAvailable = seat.IsAvailable,
                        HeldByUserId = seat.HeldByUserId
                    };
                    await _context.SaveChangesAsync();
                    await Clients.All.SendAsync("SeatStatusUpdated", statusReturn, seatData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
