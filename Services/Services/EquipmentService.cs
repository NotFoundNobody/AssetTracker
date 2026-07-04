using AssetTracker.Data;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
namespace Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly AppDbContext _db;

        public EquipmentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Equipment>> GetAllAsync()
        {
            return null;
        }
    }
}
