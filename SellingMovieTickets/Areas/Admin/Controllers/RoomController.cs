using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Movie;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.News;
using SellingMovieTickets.Areas.Admin.Models.ViewModels.Room;
using SellingMovieTickets.Models.Entities;
using SellingMovieTickets.Models.Enum;
using SellingMovieTickets.Repository;
using System.Security.Claims;

namespace SellingMovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoomController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public RoomController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchText, int pg)
        {
            IEnumerable<RoomModel> rooms = await _context.Rooms.OrderByDescending(x => x.Id).ToListAsync();
            const int pageSize = 10;
            if (pg < 1)
            {
                pg = 1;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                rooms = rooms.Where(x =>
                    (x.RoomNumber.Contains(searchText)));
            }

            int recsCount = rooms.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = rooms.Skip(resSkip).Take(pager.PageSize).ToList();
            var roomVM = _mapper.Map<List<RoomViewModel>>(data);

            ViewBag.Pager = pager;
            ViewBag.SearchText = searchText;
            return View(roomVM);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var room = await _context.Rooms.Where(x => x.Id == id).FirstOrDefaultAsync();
            var roomVM = new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RowNumber = room.RowNumber,
                NumberOfSeats = room.NumberOfSeats,
                StatusRoom = room.StatusRoom
            };

            return View(roomVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var room = new CreateRoom
            {
                StatusRooms = new SelectList(new List<string>
                {
                    StatusRoom.Active,
                    StatusRoom.SuspendOperation
                }),
            };
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoom room)
        {
            room.StatusRooms = new SelectList(new List<string>
                {
                    StatusRoom.Active,
                    StatusRoom.SuspendOperation
                });

            if (ModelState.IsValid)
            {
                var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
                var existingRoom = await _context.Rooms.FirstOrDefaultAsync(p => p.RoomNumber == room.RoomNumber);

                if (existingRoom != null)
                {
                    TempData["Error"] = "Số phòng đã có trong cơ sở dữ liệu";
                    return View(room);
                }

                var roomModel = new RoomModel
                {
                    RoomNumber = room.RoomNumber,
                    RowNumber = room.RowNumber,
                    NumberOfSeats = room.NumberOfSeats,
                    StatusRoom = room.SelectedStatusRoom,
                    TotalSeats = room.RowNumber * room.NumberOfSeats,
                    CreateDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CreateBy = nameEditor
                };
                _context.Rooms.Add(roomModel);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm phòng chiếu và ghế thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(room);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                TempData["Error"] = "Phòng chiếu không tồn tại";
                return RedirectToAction("Index");
            }

            var updateRoom = MapToUpdateRoom(room);
            updateRoom.StatusRooms = new SelectList(new List<string>
                {
                    StatusRoom.Active,
                    StatusRoom.SuspendOperation
                });
            return View(updateRoom);
        }

        private UpdateRoom MapToUpdateRoom(RoomModel room)
        {
            return new UpdateRoom
            {
                RoomNumber = room.RoomNumber,
                RowNumber = room.RowNumber,
                NumberOfSeats = room.NumberOfSeats,
                SelectedStatusRoom = room.StatusRoom
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRoom room)
        {
            room.StatusRooms = new SelectList(new List<string>
                {
                    StatusRoom.Active,
                    StatusRoom.SuspendOperation
                });

            var nameEditor = User.FindFirstValue(ClaimUserLogin.FullName);
            var existingRoom = await _context.Rooms.FindAsync(id);

            if (existingRoom == null)
            {
                TempData["Error"] = "Phòng chiếu không tồn tại";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingRoom.RoomNumber = room.RoomNumber;
                existingRoom.RowNumber = room.RowNumber;
                existingRoom.NumberOfSeats = room.NumberOfSeats;
                existingRoom.TotalSeats = room.RowNumber * room.NumberOfSeats;
                existingRoom.StatusRoom = room.SelectedStatusRoom;
                existingRoom.ModifiedBy = nameEditor;
                existingRoom.ModifiedDate = DateTime.Now;

                _context.Update(existingRoom);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật Phòng chiếu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Lỗi: " + string.Join(", ", errors);
                return View(room);
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CinemaShowTimeModel cinemaShowTime = await _context.CinemaShowTimes.Where(x => x.RoomId == Id).FirstOrDefaultAsync();
            if (cinemaShowTime == null)
            {
                RoomModel room = await _context.Rooms.FindAsync(Id);
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa phòng chiếu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Phòng chiếu đang được chọn cho một suất chiếu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult DeleteAll(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        var obj = _context.Rooms.Find(Convert.ToInt32(item));
                        _context.Rooms.Remove(obj);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
