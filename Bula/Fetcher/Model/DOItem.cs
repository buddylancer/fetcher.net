// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Model {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using System.Text.RegularExpressions;

    using Bula.Objects;
    using Bula.Model;

    /// <summary>
    /// Manipulating with items.
    /// </summary>
    public class DOItem : DOBase {
        /// Public constructor (overrides base constructor) 
        public DOItem (Connection connection): base(connection) {
            this.tableName = "items";
            this.idField = "i_ItemId";
        }

        /// <summary>
        /// Get item by ID.
        /// </summary>
        /// <param name="itemid">ID of the item.</param>
        /// <returns>Resulting data set.</returns>
        public override DataSet GetById(int itemid) { // overloaded
            if (itemid <= 0) return null;
            var query = Strings.Concat(
                " SELECT _this.*, s.s_SourceName FROM ", this.tableName, " _this ",
                " LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
                " WHERE _this.", this.idField, " = ? ");
            Object[] pars = ARR("SetInt", itemid);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Find item with given link.
        /// </summary>
        /// <param name="link">Link to find.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet FindItemByLink(String link) {
            return FindItemByLink(link, 0);
        }

        /// <summary>
        /// Find item with given link.
        /// </summary>
        /// <param name="link">Link to find.</param>
        /// <param name="sourceId">Source ID to find in (default = 0).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet FindItemByLink(String link, int sourceId) {
            if (link == null)
                return null;
            var query = Strings.Concat(
                " SELECT _this.", this.idField, " FROM ", this.tableName, " _this ",
                //(BLANK(source) ? null : " LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) "),
                " WHERE ", (sourceId == 0 ? null : " _this.i_SourceLink = ? AND "), "_this.s_Link = ?");
            Object[] pars = ARR();
            if (sourceId != 0)
                pars = ARR("SetInt", sourceId);
            pars = ADD(pars, ARR("SetString", link));
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Build SQL query from category name.
        /// </summary>
        /// <param name="category">Category name.</param>
        /// <returns>Appropriate SQL-query.</returns>
        public static String BuildSqlByCategory(String category) {
            if (NUL(category))
                return null;
            //category = Regex.Escape(Regex.Escape(category));
            category = CAT("[ ]", Regex.Escape(Regex.Escape(category)), "[, ]");
            return category == null ? null : CAT("concat(' ', _this.s_Category, ' ') REGEXP '", category, "'");
        }

        /// <summary>
        /// Build SQL query from categories filter.
        /// </summary>
        /// <param name="filter">Filter from the category.</param>
        /// <returns>Appropriate SQL-query.</returns>
        public static String BuildSqlByFilter(String filter) {
            if (filter == null)
                return null;

            String[] filterChunks = Strings.Split("~", filter);
            String[] includeChunks = SIZE(filterChunks) > 0 ?
                Strings.Split("\\|", filterChunks[0]) : null;
            String[] excludeChunks = SIZE(filterChunks) > 1 ?
                Strings.Split("\\|", filterChunks[1]) : null;
            var includeFilter = "";
            for (int n = 0; n < SIZE(includeChunks); n++) {
                if (includeFilter.Length != 0)
                    includeFilter += " OR ";
                includeFilter += "(_this.s_Title REGEXP '";
                    includeFilter += includeChunks[n];
                includeFilter += "' OR _this.t_FullDescription REGEXP '";
                    includeFilter += includeChunks[n];
                includeFilter += "')";
            }
            if (includeFilter.Length != 0)
                includeFilter = Strings.Concat(" (", includeFilter, ") ");

            var excludeFilter = "";
            for (int n = 0; n < SIZE(excludeChunks); n++) {
                if (!BLANK(excludeFilter))
                    excludeFilter = Strings.Concat(excludeFilter, " AND ");
                excludeFilter = Strings.Concat(excludeFilter,
                    "(_this.s_Title NOT REGEXP '", excludeChunks[n], "' AND _this.t_Description NOT REGEXP '", excludeChunks[n], "')");
            }
            if (excludeFilter.Length != 0)
                excludeFilter = Strings.Concat(" (", excludeFilter, ") ");

            var realFilter = includeFilter;
            if (excludeFilter.Length != 0)
                realFilter = CAT(realFilter, " AND ", excludeFilter);
            return realFilter;
        }

        /// <summary>
        /// Enumerate items.
        /// </summary>
        /// <param name="source">Source name to include items from (default - all sources).</param>
        /// <param name="search">Filter for the category (or empty).</param>
        /// <param name="list">Include the list No.</param>
        /// <param name="rows">List size.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItems(String source, String search, int list, int rows) { //, totalRows) {
            String query1 = Strings.Concat(
                " SELECT _this.", this.idField, " FROM ", this.tableName, " _this ",
                " LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
                " WHERE s.b_SourceActive = 1 AND _this.b_Counted = 1",
                (BLANK(source) ? null : CAT(" AND s.s_SourceName = '", source, "' ")),
                (BLANK(search) ? null : CAT(" AND (", search, ") ")),
                " ORDER BY _this.d_Date DESC, _this.", this.idField, " DESC "
            );

            Object[] pars1 = ARR();
            DataSet ds1 = this.GetDataSetList(query1, pars1, list, rows); //, totalRows);
            if (ds1.GetSize() == 0)
                return ds1;

            var totalPages = ds1.GetTotalPages();
            var inList = "";
            for (int n = 0; n < ds1.GetSize(); n++) {
                var o = ds1.GetRow(n);
                if (n != 0)
                    inList += ", ";
                var id = o[this.idField];
                inList += STR(id);
            }

            String query2 = Strings.Concat(
                " SELECT _this.", this.idField, ", s.s_SourceName, _this.s_Title, _this.s_Url, _this.d_Date, _this.s_Category, ",
                " _this.s_Creator, _this.s_Custom1, _this.s_Custom2 ",
                " FROM ", this.tableName, " _this ",
                " LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink ) ",
                " WHERE _this.", this.idField, " IN (", inList, ") ",
                " ORDER BY _this.d_Date DESC, _this.", this.idField, " DESC "
            );
            Object[] pars2 = ARR();
            DataSet ds2 = this.GetDataSet(query2, pars2);
            ds2.SetTotalPages(totalPages);

            return ds2;
        }

        /// <summary>
        /// Enumerate items from date.
        /// </summary>
        /// <param name="fromdate">Date to include items starting from.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItemsFromDate(String fromdate) {
            var query = Strings.Concat(
                " SELECT _this.*, s.s_SourceName FROM ", this.tableName, " _this ",
                " INNER JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
                " WHERE _this.b_Counted = 1 AND _this.d_Date > ? ",
                " ORDER BY _this.d_Date DESC, _this.", this.idField, " DESC "
            );
            Object[] pars = ARR("SetDate", fromdate);
            return this.GetDataSet(query, pars);

        }

        /// <summary>
        /// Enumerate items from given date.
        /// </summary>
        /// <param name="fromDate">Date to include items starting from.</param>
        /// <param name="source">Source name to include items from (default - all sources).</param>
        /// <param name="filter">Filter for the category (or empty - no filtering).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItemsFromSource(String fromDate, String source, String filter) {
            return this.EnumItemsFromSource(fromDate, source, filter, 20);
        }

        /// <summary>
        /// Enumerate items from given date.
        /// </summary>
        /// <param name="fromDate">Date to include items starting from.</param>
        /// <param name="source">Source name to include items from (default - all sources).</param>
        /// <param name="search">Search for filtering category (or empty - no filtering).</param>
        /// <param name="maxItems">Max number of returned items.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItemsFromSource(String fromDate, String source, String search, int maxItems) {
            String query1 = Strings.Concat(
                " SELECT _this.*, s.s_SourceName FROM ", this.tableName, " _this ",
                " INNER JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
                " WHERE s.b_SourceActive = 1 AND _this.b_Counted = 1",
                (BLANK(source) ? null : Strings.Concat(" AND s.s_SourceName = '", source, "' ")),
                (BLANK(search) ? null : Strings.Concat(" AND (", search, ") ")),
                " ORDER BY _this.d_Date DESC, _this.", this.idField, " DESC ",
                " LIMIT ", STR(maxItems)
            );
            Object[] pars1 = ARR();
            DataSet ds1 = this.GetDataSet(query1, pars1);
            if (fromDate == null)
                return ds1;

            String query2 = Strings.Concat(
                " SELECT _this.*, s.s_SourceName FROM ", this.tableName, " _this ",
                " INNER JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
                " WHERE s.b_SourceActive = 1 ",
                (BLANK(source) ? null : Strings.Concat(" AND s.s_SourceName = '", source, "' ")),
                " AND _this.d_Date > ? ",
                (BLANK(search) ? null : Strings.Concat(" AND (", search, ") ")),
                " ORDER BY _this.d_Date DESC, _this.", this.idField, " DESC ",
                " LIMIT ", STR(maxItems)
            );
            Object[] pars2 = ARR("SetDate", fromDate);
            DataSet ds2 = this.GetDataSet(query2, pars2);

            return ds1.GetSize() > ds2.GetSize() ? ds1 : ds2;
        }

        /// <summary>
        /// Purge items.
        /// </summary>
        /// <param name="days">Remove items older than days.</param>
        /// <returns>Resulting data set.</returns>
        public int PurgeOldItems(int days) {
            var purgeDate = DateTimes.Format(DBConfig.SQL_DTS, DateTimes.GetTime(CAT("-", days, " days")));
            var query = Strings.Concat("DELETE FROM ", this.tableName, " WHERE d_Date < ?");
            Object[] pars = ARR("SetDate", purgeDate);

            return this.UpdateInternal(query, pars, "update");
        }
    }
}