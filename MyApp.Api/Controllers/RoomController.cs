using KapadaModel.DBOs;
using KapadaModel.Item;
using KapadaModel.Room;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Api.DataContext;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("createRoom")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomDBO _room)
        {
            var existingRoom = await _context.RoomName.FirstOrDefaultAsync(i => i.RoomName == _room.RoomName);
            if (existingRoom != null)
            {
                return BadRequest("Room name already exists");
            }

            var newRoom = new RoomItem
            {
                RoomName = _room.RoomName
            };

            _context.RoomName.Add(newRoom);
            await _context.SaveChangesAsync();

            return Ok("Room created successfully");
        }

        [HttpGet("getRoom")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.RoomName
                .Select(c => new RoomResponseDBO
                {
                    Id = c.Id,
                    RoomName = c.RoomName ?? string.Empty
                })
                .ToListAsync();

            return Ok(rooms);
        }

        [HttpPut("updateRoom/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomDBO updatedRoom)
        {
            var room = await _context.RoomName.FindAsync(id);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            room.RoomName = updatedRoom.RoomName;
            await _context.SaveChangesAsync();

            return Ok("RoomName updated successfully.");
        }

        [HttpDelete("deleteRoom/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.RoomName.FindAsync(id);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            _context.RoomName.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Room Deleted Successfully");
        }
    }
}
