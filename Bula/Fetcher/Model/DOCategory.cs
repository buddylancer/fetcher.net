// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Model {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Objects;
    using Bula.Model;

    /// <summary>
    /// Manipulating with categories.
    /// </summary>
    public class DOCategory : DOBase {
        /// Public constructor (overrides base constructor) 
        public DOCategory (Connection connection): base(connection) {
            this.tableName = "categories";
            this.idField = "s_CatId";
        }

        /// <summary>
        /// Get category by ID.
        /// </summary>
        /// <param name="catid">Category ID.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetCategoryById(String catid) {
            if (BLANK(catid))
                return null;
            var query = Strings.Concat(
                " SELECT * FROM ", this.tableName, " _this " ,
                " WHERE _this.", this.idField, " = ? ");
            Object[] pars = ARR("SetString", catid);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get category by name.
        /// </summary>
        /// <param name="catname">Category name.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetCategoryByName(String catname) {
            if (BLANK(catname))
                return null;
            var query = Strings.Concat(
                " SELECT * FROM ", this.tableName, " _this ",
                " WHERE _this.s_Name = ? ");
            Object[] pars = ARR("SetString", catname);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Enumerate categories.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories() {
            return this.EnumCategories(null, 0, 0); }

        /// <summary>
        /// Enumerate categories.
        /// </summary>
        /// <param name="order">Field name to sort result by (default = null).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories(String order) {
            return this.EnumCategories(order, 0, 0); }

        /// <summary>
        /// Enumerate categories.
        /// </summary>
        /// <param name="order">Field name to sort result by (default = null).</param>
        /// <param name="minCount">Include categories with Counter >= min_count.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories(String order, int minCount) {
            return this.EnumCategories(order, minCount, 0); }

        /// <summary>
        /// Enumerate categories.
        /// </summary>
        /// <param name="order">Field name to sort result by (default = null).</param>
        /// <param name="minCount">Include categories with Counter >= min_count.</param>
        /// <param name="limit">Include not more than "limit" records (default = no limit).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories(String order, int minCount, int limit) {
            if (minCount < 0)
                return null;
            var query = Strings.Concat(
                " SELECT * FROM ", this.tableName, " _this ",
                (minCount > 0 ? CAT(" WHERE _this.i_Counter > ", minCount) : null),
                " ORDER BY ", (EQ(order, "counter") ? " _this.i_Counter desc " : " _this.s_CatId asc "),
                (limit == 0 ? null : CAT(" LIMIT ", limit))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Check whether category (filter) exists.
        /// </summary>
        /// <param name="filterName">Category ID.</param>
        /// <param name="category">Category object (if found) copied to element 0 of object array.</param>
        /// <returns>True if exists.</returns>
        public Boolean CheckFilterName(String filterName, Object[]category) {
            var dsCategories = this.Select();
            var filterFound = false;
            for (int n = 0; n < dsCategories.GetSize(); n++) {
                var oCategory = dsCategories.GetRow(n);
                if (EQ(oCategory["s_CatId"], filterName)) {
                    filterFound = true;
                    if (category != null)
                        category[0] = oCategory;
                    break;
                }
            }
            return filterFound;
        }
    }
}