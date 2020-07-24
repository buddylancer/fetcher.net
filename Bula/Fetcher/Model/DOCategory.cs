namespace Bula.Fetcher.Model {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Objects;
    using Bula.Model;

    /**
     * Manipulating with categories.
     */
    public class DOCategory : DOBase {

        public DOCategory (): base() {
            this.table_name = "categories";
            this.id_field = "s_CatId";
        }

        ///Get category by ID.
        /// <param name="catid">Category ID.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetCategoryById(String catid) {
            if (BLANK(catid))
                return null;
            String query = Strings.Concat(
                " SELECT * FROM ", this.table_name, " _this " ,
                " WHERE _this.", this.id_field, " = ? ");
            Object[] pars = ARR("SetString", catid);
            return this.GetDataSet(query, pars);
        }

        ///Get category by name.
        /// <param name="catname">Category name.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetCategoryByName(String catname) {
            if (BLANK(catname))
                return null;
            String query = Strings.Concat(
                " SELECT * FROM ", this.table_name, " _this ",
                " WHERE _this.s_Name = ? ");
            Object[] pars = ARR("SetString", catname);
            return this.GetDataSet(query, pars);
        }

        ///Enumerate categories.
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories() {
            return this.EnumCategories(null, 0, 0); }

        ///Enumerate categories.
        /// <param name="order">Field name to sort result by (default = null).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories(String order) {
            return this.EnumCategories(order, 0, 0); }

        ///Enumerate categories.
        /// <param name="order">Field name to sort result by (default = null).</param>
        /// <param name="min_count">Include categories with Counter >= min_count.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories(String order, int min_count) {
            return this.EnumCategories(order, min_count, 0); }

        ///Enumerate categories.
        /// <param name="order">Field name to sort result by (default = null).</param>
        /// <param name="min_count">Include categories with Counter >= min_count.</param>
        /// <param name="limit">Include not more than "limit" records (default = no limit).</param>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumCategories(String order, int min_count, int limit) {
            if (min_count < 0)
                return null;
            String query = Strings.Concat(
                " SELECT * FROM ", this.table_name, " _this ",
                (min_count > 0 ? CAT(" WHERE _this.i_Counter > ", min_count) : null),
                " ORDER BY ", (EQ(order, "counter") ? " _this.i_Counter desc " : " _this.s_CatId asc "),
                (limit == 0 ? null : CAT(" LIMIT ", limit))
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }
    }
}