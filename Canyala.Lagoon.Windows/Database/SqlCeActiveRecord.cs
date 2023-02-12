//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
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
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Canyala.Lagoon.Core.Extensions;
using Canyala.Lagoon.Core.Functional;
using Canyala.Lagoon.Windows.Database.Internal;
using System.Data.OleDb;
using System.Data;

namespace Canyala.Lagoon.Windows.Database;

/// <summary>
/// Provides a stateless, record based data access layer (DAL) for MS-SQL CE databases
/// with engine predicate execution implemented using query expressions.
/// </summary>
public class SqlCeActiveRecord
{
    private readonly string _connectionString;

    private SqlCeActiveRecord(string connectionString)
    { _connectionString = connectionString; }

    /// <summary>
    /// Creates a MS-SQL DAO.
    /// </summary>
    /// <param name="connectionString">A SQL connection string.</param>
    /// <returns>A sql DAO instance.</returns>
    public static SqlCeActiveRecord FromConnectionString(string connectionString)
    { return new SqlCeActiveRecord(connectionString); }

    /// <summary>
    /// Creates a table in the database using a record class as a template.
    /// </summary>
    /// <typeparam name="T">The record class to use as a template</typeparam>
    public void CreateTable<T>()
    {
        ExecuteWriteQuery(Query.CreateTable<T>(), command => command.ExecuteNonQuery());
        _tableNames.Add(Query.TableName(typeof(T)));
    }

    private readonly HashSet<string> _tableNames = new();

    /// <summary>
    /// Verifies that a table exist.
    /// </summary>
    /// <typeparam name="T">The record class to use as a template.</typeparam>
    /// <returns><code>true</code> if the table exists, otherwize <code>false</code>.</returns>
    public bool VerifyTable<T>()
    {
        const string TABLE_NAME = "TABLE_NAME";

        using var connection = new SqlCeConnection(_connectionString);

        try
        {
            connection.Open();
            var table = connection.GetSchema("Tables");

            foreach (DataRow row in table.Rows)
            {
                var tableName = row[TABLE_NAME].ToString();

                if (tableName is null)
                {
                        throw new IndexOutOfRangeException($"Table '{TABLE_NAME}' not present in Database '{_connectionString}'.");
                }

                _ = _tableNames.Add(tableName);
            }
        }
        finally
        {
            connection.Close();
        }

        return _tableNames.Contains(Query.TableName(typeof(T)));
    }

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
    public int Count<T>(Expression<Predicate<T>>? expression = null)
    { return ExecuteWriteQuery(Query.Count(expression), command => (int)command.ExecuteScalar()); }

    /// <summary>
    /// Updates a record, requires that the record type declares an Id of type Guid
    /// </summary>
    /// <typeparam name="T">The type of a record, corresponds to a table.</typeparam>
    /// <param name="record">The record.</param>
    /// <returns>This instance.</returns>
    public SqlCeActiveRecord Update<T>(T record) where T : IdentityRecord, new()
    { return Update(Seq.Of(record)); }

    /// <summary>
    /// Updates a sequence of records, requires that the record type declares an Id of type Guid
    /// </summary>
    /// <typeparam name="T">The type of a record, corresponds to a table.</typeparam>
    /// <param name="records">The records.</param>
    /// <returns>This instance.</returns>
    public SqlCeActiveRecord Update<T>(IEnumerable<T> records) where T : IdentityRecord, new()
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
    public SqlCeActiveRecord Insert<T>(T record) where T : new()
    { return Insert(Seq.Of(record)); }

    /// <summary>
    /// Insert a sequence of records into the database.
    /// </summary>
    /// <typeparam name="T">The record type, corresponds to a table.</typeparam>
    /// <param name="record">The record to insert.</param>
    /// <returns>This instance.</returns>
    public SqlCeActiveRecord Insert<T>(IEnumerable<T> records) where T : new()
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
    public int Delete<T>(Expression<Predicate<T>>? wherePredicate = null) where T : new()
    { return ExecuteWriteQuery(Query.Delete(wherePredicate), commandFunction => commandFunction.ExecuteNonQuery()); }

    /// <summary>
    /// Selects records that match a predicate condition.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    /// <param name="wherePredicate">The where predicate specifying a condition.</param>
    /// <param name="orderByAccessor">An order by accessor</param>
    /// <param name="sortDirection">Direction to sort orderby with.</param>
    /// <returns>Records of type T</returns>
    public IEnumerable<TResult> Select<T, TResult>(Expression<Predicate<T>>? where = null, Expression<Func<TResult, object>>? orderBy = null, Sort sort = Sort.Ascending, int skip = 0, int take = 0)
        where TResult : new()
        where T : new()
    {
        using var connection = new SqlCeConnection(_connectionString);
        var query = Query.Select(where, orderBy, sort, skip, take);
        var command = new SqlCeCommand(query, connection);

        try
        {
            connection.Open();

            using var reader = command.ExecuteReader();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (reader.Read())
            {
                var record = new TResult();
                properties.Do(property => property.SetValue(record, reader[Query.ColumnName(property)]));
                yield return record;
            }
        }
        finally
        {
            connection.Close();
        }
    }

    public IEnumerable<T> Select<T>(Expression<Predicate<T>>? where = null, Expression<Func<T, object>>? orderBy = null, Sort sort = Sort.Ascending, int skip = 0, int take = 0)
        where T : new()
    {
        return Select<T, T>(where, orderBy, sort, skip, take);
    }

    private int ExecuteWriteQuery(string query, Func<SqlCeCommand, int> actionFunction)
    {
        using var connection = new SqlCeConnection(_connectionString);
        var command = new SqlCeCommand(query, connection);

        try
        {
            connection.Open();
            return actionFunction(command);
        }
        catch (Exception)
        {
            return 0;
        }
        finally
        {
            connection.Close();
        }
    }
}
