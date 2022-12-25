//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//-------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Canyala.Lagoon.Extensions;
using Canyala.Lagoon.Functional;
using Canyala.Lagoon.Database.Internal;
using System.Data.OleDb;

namespace Canyala.Lagoon.Database
{
    /// <summary>
    /// Provides a stateless, record based data access layer (DAL) for MS-SQL databases
    /// with serverside predicate excecution implemented using query expressions.
    /// </summary>
    public class SqlActiveRecord
    {
        private readonly string _connectionString;

        private SqlActiveRecord(string connectionString)
            { _connectionString = connectionString; }

        /// <summary>
        /// Creates a MS-SQL DAO.
        /// </summary>
        /// <param name="connectionString">A SQL connection string.</param>
        /// <returns>A sql DAO instance.</returns>
        public static SqlActiveRecord FromConnectionString(string connectionString)
            { return new SqlActiveRecord(connectionString); }

        /// <summary>
        /// Creates a table in the database using a record class as a template.
        /// </summary>
        /// <typeparam name="T">The record class to use as a template</typeparam>
        public void CreateTable<T>()
            { ExecuteWriteQuery(Query.CreateTable<T>(), command => command.ExecuteNonQuery()); }

        /// <summary>
        /// Drops a table from database using a record class as a template.
        /// </summary>
        /// <typeparam name="T">The record class to use as a template</typeparam>
        public void DropTable<T>()
            { ExecuteWriteQuery(Query.DropTable<T>(), command => command.ExecuteNonQuery()); }

        /// <summary>
        /// Executes a serverside count.
        /// </summary>
        /// <typeparam name="T">The record type, corresponds to a table.</typeparam>
        /// <param name="expression">The predicate expression to use as a condition for the count.</param>
        /// <returns></returns>
        public int Count<T>(Expression<Predicate<T>> expression = null)
            { return ExecuteWriteQuery(Query.Count(expression), command => (int) command.ExecuteScalar()); }

        /// <summary>
        /// Verifies that a table exist for the record type.
        /// </summary>
        /// <typeparam name="T">The record class to use as a template</typeparam>
        public bool VerifyTable<T>()
            { return ExecuteWriteQuery(Query.VerifyTable<T>(), command => (int) command.ExecuteScalar()) > 0; }

        /// <summary>
        /// Updates a record, requires that the record type declares an Id of type Guid
        /// </summary>
        /// <typeparam name="T">The type of a record, corresponds to a table.</typeparam>
        /// <param name="record">The record.</param>
        /// <returns>This instance.</returns>
        public SqlActiveRecord Update<T>(T record) where T : IdentityRecord,  new()
            { return Update(Seq.Of(record)); }

        /// <summary>
        /// Updates a sequence of records, requires that the record type declares an Id of type Guid
        /// </summary>
        /// <typeparam name="T">The type of a record, corresponds to a table.</typeparam>
        /// <param name="records">The records.</param>
        /// <returns>This instance.</returns>
        public SqlActiveRecord Update<T>(IEnumerable<T> records) where T : IdentityRecord, new()
        { 
            ExecuteWriteQuery(Query.Update(records), command => command.ExecuteNonQuery());
            return this;
        }

        /// <summary>
        /// Insert a record into the database.
        /// </summary>
        /// <typeparam name="T">The record type, corresponds to a table.</typeparam>
        /// <param name="record">The record to insert.</param>
        /// <returns>This instance.</returns>
        public SqlActiveRecord Insert<T>(T record) where T : new()
            { return Insert(Seq.Of(record)); }

        /// <summary>
        /// Insert a sequence of records into the database.
        /// </summary>
        /// <typeparam name="T">The record type, corresponds to a table.</typeparam>
        /// <param name="record">The record to insert.</param>
        /// <returns>This instance.</returns>
        public SqlActiveRecord Insert<T>(IEnumerable<T> records) where T : new()
        {
            ExecuteWriteQuery(Query.Insert(records), command => command.ExecuteNonQuery());
            return this;
        }

        /// <summary>
        /// Deletes records that match a predicate condition.
        /// </summary>
        /// <typeparam name="T">The record type</typeparam>
        /// <param name="wherePredicate">The predicate condition.</param>
        /// <returns>Number of rows affected.</returns>
        public int Delete<T>(Expression<Predicate<T>> wherePredicate = null) where T : new()
            { return ExecuteWriteQuery(Query.Delete(wherePredicate), commandFunction => commandFunction.ExecuteNonQuery()); }

        /// <summary>
        /// Selects records that match a predicate condition.
        /// </summary>
        /// <typeparam name="T">The record type.</typeparam>
        /// <param name="wherePredicate">The where predicate specifying a condition.</param>
        /// <param name="orderByAccessor">An order by accessor</param>
        /// <param name="sortDirection">Direction to sort orderby with.</param>
        /// <returns>Records of type T</returns>
        public IEnumerable<T> Select<T>(Expression<Predicate<T>> where=null, Expression<Func<T,object>> orderBy=null, Sort sort=Sort.Ascending, int skip=0, int take=0)
            where T : new()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = Query.Select(where, orderBy, sort, skip, take);
                var command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var properties = typeof(T).GetProperties();

                        while (reader.Read())
                        {
                            var record = new T();
                            properties.Do(property => property.SetValue(record, reader[Query.ColumnName(property)]));
                            yield return record;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private int ExecuteWriteQuery(string query, Func<SqlCommand,int> actionFunction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    return actionFunction(command);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
