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
    /// Manipulations with sources.
    /// </summary>
    public class DOSource : DOBase {
        /// Public constructor (overrides base constructor) 
        public DOSource (Connection connection): base(connection) {
            this.tableName = "sources";
            this.idField = "i_SourceId";
        }

        /// <summary>
        /// Enumerates all sources.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumSources() {
            var query = Strings.Concat(
                " SELECT _this.* FROM ", this.tableName, " _this ",
                " where _this.b_SourceActive = 1 ",
                " order by _this.s_SourceName asc"
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Enumerates sources, which are active for fetching.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumFetchedSources() {
            var query = Strings.Concat(
                " SELECT _this.* FROM ", this.tableName, " _this ",
                " where _this.b_SourceFetched = 1 ",
                " order by _this.s_SourceName asc"
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Enumerates all sources with counters.
        /// </summary>
        /// <returns>Resulting data set.</returns>
        public DataSet EnumSourcesWithCounters() {
            var query = Strings.Concat(
                " select _this.", this.idField, ", _this.s_SourceName, ",
                " Count(p.i_SourceLink) as cntpro ",
                " from ", this.tableName, " _this ",
                " left outer join items p on (p.i_SourceLink = _this.i_SourceId) ",
                " where _this.b_SourceActive = 1 ",
                " group by _this.i_SourceId ",
                " order by _this.s_SourceName asc "
            );
            Object[] pars = ARR();
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get source by ID.
        /// </summary>
        /// <param name="sourceid">Source ID.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetSourceById(int sourceid) {
            if (sourceid <= 0) return null;
            var query = Strings.Concat("SELECT * FROM sources where i_SourceId = ?");
            Object[] pars = ARR("SetInt", sourceid);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Get source by name.
        /// </summary>
        /// <param name="sourcename">Source name.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetSourceByName(String sourcename) {
            if (sourcename == null || sourcename == "") return null;
            var query = Strings.Concat("SELECT * FROM sources where s_SourceName = ?");
            Object[] pars = ARR("SetString", sourcename);
            return this.GetDataSet(query, pars);
        }

        /// <summary>
        /// Check whether source exists.
        /// </summary>
        /// <param name="sourcename">Source name.</param>
        /// <returns>True if exists.</returns>
        public Boolean CheckSourceName(String sourcename) {
            return CheckSourceName(sourcename, null);
        }

        /// <summary>
        /// Check whether source exists.
        /// </summary>
        /// <param name="sourcename">Source name.</param>
        /// <param name="source">Source object (if found) copied to element 0 of object array.</param>
        /// <returns>True if exists.</returns>
        public Boolean CheckSourceName(String sourcename, Object[]source) {
            var dsSources = this.EnumSources();
            var sourceFound = false;
            for (int n = 0; n < dsSources.GetSize(); n++) {
                var oSource = dsSources.GetRow(n);
                if (EQ(oSource["s_SourceName"], sourcename)) {
                    sourceFound = true;
                    if (source != null)
                        source[0] = oSource;
                    break;
                }
            }
            return sourceFound;
        }
    }
}