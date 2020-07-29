namespace Bula.Fetcher.Model {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Objects;
    using Bula.Model;

    /// <summary>
    /// Manipulating with items.
    /// </summary>
    public class DOItem : DOBase {
        /// Public constructor (overrides base constructor) 
        public DOItem (): base() {
            this.table_name = "items";
            this.id_field = "i_ItemId";
        }

        /// <summary>
        /// Get item by ID.
        /// </summary>
        /// <param name="itemid">ID of the item.</param>
        /// <returns>Resulting data set.</returns>
        public override DataSet GetById(int itemid) { // overloaded
            if (itemid <= 0) return null;
            var query = Strings.Concat(
                " SELECT _this.*, s.s_SourceName FROM ", this.table_name, " _this ",
                " LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
                " WHERE _this.", this.id_field, " = ? ");
            Object[] pars = ARR("SetInt", itemid);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Find item with given link.
        /// </summary>
        /// <param name="link">Link to find.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet FindItemByLink(String link) {
            return FindItemByLink(link, 0); }

        /// <summary>
        /// Find item with given link.
        /// </summary>
        /// <param name="link">Link to find.</param>
        /// <param name="source_id">Source ID to find in (default = 0).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet FindItemByLink(String link, int source_id) {
            if (link == null)
                return null;
            var query = Strings.Concat(
                " SELECT _this.", this.id_field, " FROM ", this.table_name, " _this ",
                //(BLANK(source) ? null : " LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) "),
                " WHERE ", (source_id == 0 ? null : " _this.i_SourceLink = ? AND "), "_this.s_Link = ?");
            Object[] pars = ARR();
            if (source_id != 0)
                pars = ARR("SetInt", source_id);
            pars = ADD(pars, ARR("SetString", link));
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Build SQL query from categories filter.
        /// </summary>
        /// <param name="filter">Filter from the category.</param>
        /// <returns>Appropriate SQL-query.</returns>
        public String BuildSqlFilter(String filter) {
            String[] filter_chunks = Strings.Split("~", filter);
            String[] include_chunks = SIZE(filter_chunks) > 0 ?
                Strings.Split("|", filter_chunks[0]) : null;
            String[] exclude_chunks = SIZE(filter_chunks) > 1 ?
                Strings.Split("|", filter_chunks[1]) : null;
            var include_filter = "";
            for (int n = 0; n < SIZE(include_chunks); n++) {
                if (include_filter.Length != 0)
                    include_filter += (" OR ");
                include_filter += ("(_this.s_Title LIKE '%");
                    include_filter += (include_chunks[n]);
    			include_filter += ("%' OR _this.t_FullDescription LIKE '%");
    				include_filter += (include_chunks[n]);
    			include_filter += ("%')");
    		}
    		if (include_filter.Length != 0)
    			include_filter = Strings.Concat(" (", include_filter, ") ");

    		var exclude_filter = "";
    		for (int n = 0; n < SIZE(exclude_chunks); n++) {
    			if (!BLANK(exclude_filter))
    				exclude_filter = Strings.Concat(exclude_filter, " AND ");
    			exclude_filter = Strings.Concat(exclude_filter,
    				"(_this.s_Title not like '%", exclude_chunks[n], "%' AND _this.t_Description not like '%", exclude_chunks[n], "%')");
    		}
    		if (exclude_filter.Length != 0)
    			exclude_filter = Strings.Concat(" (", exclude_filter, ") ");

    		var real_filter = include_filter;
    		if (exclude_filter.Length != 0)
    			real_filter = CAT(real_filter, " AND ", exclude_filter);
    		return real_filter;
    	}

    	/// <summary>
    	/// Enumerate items.
    	/// </summary>
        /// <param name="source">Source name to include items from (default - all sources).</param>
        /// <param name="search">Filter for the category (or empty).</param>
        /// <param name="list">Include the list No.</param>
        /// <param name="rows">List size.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItems(String source, String search, int list, int rows) { //, total_rows) {
    		var real_search = BLANK(search) ? null : this.BuildSqlFilter(search);
    		String query1 = Strings.Concat(
    			" SELECT _this.", this.id_field, " FROM ", this.table_name, " _this ",
    			" LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
    			" WHERE s.b_SourceActive = 1 ",
    			(BLANK(source) ? null : CAT(" AND s.s_SourceName = '", source, "' ")),
    			(BLANK(real_search) ? null : CAT(" AND (", real_search, ") ")),
    			" ORDER BY _this.d_Date DESC, _this.", this.id_field, " DESC "
    		);

    		Object[] pars1 = ARR();
    		DataSet ds1 = this.GetDataSetList(query1, pars1, list, rows); //, total_rows);
    		if (ds1.GetSize() == 0)
    			return ds1;

    		var total_pages = ds1.GetTotalPages();
    		var in_list = "";
    		for (int n = 0; n < ds1.GetSize(); n++) {
    			var o = ds1.GetRow(n);
    			if (n != 0)
    				in_list += (", ");
                var id = o[this.id_field];
    			in_list += (id);
    		}

    		String query2 = Strings.Concat(
    			" SELECT _this.", this.id_field, ", s.s_SourceName, _this.s_Title, _this.s_Url, _this.d_Date, _this.s_Category, ",
    			" _this.s_Creator, _this.s_Custom1, _this.s_Custom2, s.s_SourceName ",
    			" FROM ", this.table_name, " _this ",
    			" LEFT JOIN sources s ON (s.i_SourceId = _this.i_SourceLink ) ",
    			" WHERE _this.", this.id_field, " IN (", in_list, ") ",
    			" ORDER BY _this.d_Date DESC, _this.", this.id_field, " DESC "
    		);
    		Object[] pars2 = ARR();
    		DataSet ds2 = this.GetDataSet(query2, pars2);
    		ds2.SetTotalPages(total_pages);

    		return ds2;
    	}

        /// <summary>
        /// Enumerate items from date.
        /// </summary>
        /// <param name="fromdate">Date to include items starting from.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItemsFromDate(String fromdate) {
    		var query = Strings.Concat(
    			" SELECT _this.*, s.s_SourceName FROM ", this.table_name, " _this ",
    			" INNER JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
    			" WHERE _this.d_Date > ? ",
    			" ORDER BY _this.d_Date DESC, _this.", this.id_field, " DESC "
    		);
    		Object[] pars = ARR("SetDate", fromdate);
    		return this.GetDataSet(query, pars);

    	}

    	/// <summary>
    	/// Enumerate items from given date.
    	/// </summary>
        /// <param name="from_date">Date to include items starting from.</param>
        /// <param name="source">Source name to include items from (default - all sources).</param>
        /// <param name="filter">Filter for the category (or empty - no filtering).</param>
        /// <returns>Resulting data set.</returns>
    	public DataSet EnumItemsFromSource(String from_date, String source, String filter) {
            return this.EnumItemsFromSource(from_date, source, filter, 20);
        }

    	/// <summary>
    	/// Enumerate items from given date.
    	/// </summary>
        /// <param name="from_date">Date to include items starting from.</param>
        /// <param name="source">Source name to include items from (default - all sources).</param>
        /// <param name="filter">Filter for the category (or empty - no filtering).</param>
        /// <param name="max_items">Max number of returned items.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumItemsFromSource(String from_date, String source, String filter, int max_items) {
    		var real_filter = BLANK(filter) ? null : this.BuildSqlFilter(filter);
    		String query1 = Strings.Concat(
    			" SELECT _this.*, s.s_SourceName FROM ", this.table_name, " _this ",
    			" INNER JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
    			" WHERE s.b_SourceActive = 1 ",
    			(BLANK(source) ? null : Strings.Concat(" AND s.s_SourceName = '", source, "' ")),
    			(BLANK(real_filter) ? null : Strings.Concat(" AND (", real_filter, ") ")),
    			" ORDER BY _this.d_Date DESC, _this.", this.id_field, " DESC ",
    			" LIMIT ", max_items
    		);
    		Object[] pars1 = ARR();
    		DataSet ds1 = this.GetDataSet(query1, pars1);
            if (from_date == null)
                return ds1;

    		String query2 = Strings.Concat(
    			" SELECT _this.*, s.s_SourceName FROM ", this.table_name, " _this ",
    			" INNER JOIN sources s ON (s.i_SourceId = _this.i_SourceLink) ",
    			" WHERE s.b_SourceActive = 1 ",
    			(BLANK(source) ? null : Strings.Concat(" AND s.s_SourceName = '", source, "' ")),
    			" AND _this.d_Date > ? ",
    			(BLANK(real_filter) ? null : Strings.Concat(" AND (", real_filter, ") ")),
    			" ORDER BY _this.d_Date DESC, _this.", this.id_field, " DESC ",
    			" LIMIT ", max_items
    		);
    		Object[] pars2 = ARR("SetDate", from_date);
    		DataSet ds2 = this.GetDataSet(query2, pars2);

    		return ds1.GetSize() > ds2.GetSize() ? ds1 : ds2;
    	}

        /// <summary>
        /// Purge items.
        /// </summary>
        /// <param name="days">Remove items older than days.</param>
        /// <returns>Resulting data set.</returns>
        public int PurgeOldItems(int days) {
    		var purge_date = DateTimes.Format(DBConfig.SQL_DTS, DateTimes.GetTime(CAT("-", days, " days")));
    		var query = Strings.Concat("DELETE FROM ", this.table_name, " WHERE d_Date < ?");
    		Object[] pars = ARR("SetDate", purge_date);

    		return this.UpdateInternal(query, pars, "update");
    	}
    }
}