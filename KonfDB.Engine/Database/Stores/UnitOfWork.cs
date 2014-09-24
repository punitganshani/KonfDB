#region License and Product Information

// 
//     This file 'UnitOfWork.cs' is part of KonfDB application - 
//     a project perceived and developed by Punit Ganshani.
// 
//     KonfDB is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     KonfDB is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with KonfDB.  If not, see <http://www.gnu.org/licenses/>.
// 
//     You can also view the documentation and progress of this project 'KonfDB'
//     on the project website, <http://www.konfdb.com> or on 
//     <http://www.ganshani.com/applications/konfdb>

#endregion

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using KonfDB.Engine.Database.EntityFramework;
using KonfDB.Infrastructure.Database.Stores;

namespace KonfDB.Engine.Database.Stores
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _changesSaved;

        private readonly KonfDBEntities _context;

        public KonfDBEntities Context
        {
            get { return _context; }
        }

        public UnitOfWork(string connectionString)
        {
            _context = new KonfDBEntities(connectionString);
        }

        public void SaveChanges()
        {
            var entries = _context.ChangeTracker.Entries();

            int noOfChanges = _context.SaveChanges();
            _changesSaved = noOfChanges > 0;
        }

        public void Add<TE>(TE entity) where TE : class
        {
            try
            {
                _context.Entry(entity).State = EntityState.Added;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public void Update<TE>(TE entity) where TE : class
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public void Delete<TE>(TE entity) where TE : class
        {
            try
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public void Update<TEntity, TUserInput>(ICollection<TEntity> databaseTable,
            List<TUserInput> userInput,
            Func<TEntity, long> GetUniqueIdFromDB,
            Func<TUserInput, long> GetUniqueIdFromUserInput,
            Func<TUserInput, TEntity> ConvertToDbModel)
            where TEntity : class
            //where TUserInput : BaseViewModel
        {
            List<long> previousDataInDB = databaseTable
                .Where(ep => userInput.Any(ui => GetUniqueIdFromUserInput(ui) == GetUniqueIdFromDB(ep)))
                .Select(ep => GetUniqueIdFromDB(ep))
                .ToList();

            List<long> currentDataInDb = databaseTable
                .Select(o => GetUniqueIdFromDB(o))
                .ToList();

            List<long> toBeDeletedFromDB = previousDataInDB
                .Except(currentDataInDb).ToList();
            // Delete
            toBeDeletedFromDB.ForEach(x =>
            {
                TEntity entityInDb = databaseTable.FirstOrDefault(record => GetUniqueIdFromDB(record) == x);
                if (entityInDb != null)
                {
                    Context.Entry(entityInDb).State = EntityState.Deleted;
                }
            });

            // Add New, or Modify
            userInput.ForEach(x =>
            {
                if (GetUniqueIdFromUserInput(x) == -1) // Not added in DB
                {
                    Add(ConvertToDbModel(x));
                }
                else
                {
                    TEntity entityInDb =
                        databaseTable.FirstOrDefault(record => GetUniqueIdFromDB(record) == GetUniqueIdFromUserInput(x));
                    Update(entityInDb);
                }
            });
        }

        public void Dispose()
        {
            if (!_changesSaved)
                SaveChanges();
        }
    }
}