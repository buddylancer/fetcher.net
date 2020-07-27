namespace Bula.Fetcher.Model {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Objects;
    using Bula.Model;

    /**
     * Manipulations with sources.
     */
    public class DOSource : DOBase {

    	public DOSource (): base() {
    		this.table_name = "sources";
    		this.id_field = "i_SourceId";
    	}

        ///Enumerates all sources.
        /// <returns>Resulting data set.</returns>
        public DataSet EnumSources() {
    		var query = Strings.Concat(
    			" SELECT _this.* FROM ", this.table_name, " _this ",
    			" where _this.b_SourceActive = 1 ",
    			" order by _this.s_SourceName asc"
    		);
    		Object[] pars = ARR();
    		return this.GetDataSet(query, pars);
    	}

        ///Enumerates sources, which are active for fetching.
        /// <returns>Resulting data set.</returns>
    	public DataSet EnumFetchedSources() {
    		var query = Strings.Concat(
    			" SELECT _this.* FROM ", this.table_name, " _this ",
    			" where _this.b_SourceFetched = 1 ",
    			" order by _this.s_SourceName asc"
    		);
    		Object[] pars = ARR();
    		return this.GetDataSet(query, pars);
    	}

        ///Enumerates all sources with counters.
        /// <returns>Resulting data set.</returns>
    	public DataSet EnumSourcesWithCounters() {
    		var query = Strings.Concat(
    			" select _this.", this.id_field, ", _this.s_SourceName, ",
    			" Count(p.i_SourceLink) as cntpro ",
    			" from ", this.table_name, " _this ",
    			" left outer join items p on (p.i_SourceLink = _this.i_SourceId) ",
    			" where _this.b_SourceActive = 1 ",
    			" group by _this.i_SourceId ",
    			" order by _this.s_SourceName asc "
    		);
    		Object[] pars = ARR();
    		return this.GetDataSet(query, pars);
    	}

        ///Get source by ID.
        /// <param name="sourceid">Source ID.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetSourceById(int sourceid) {
            if (sourceid <= 0) return null;
    		var query = Strings.Concat("SELECT * FROM sources where i_SourceId = ?");
    		Object[] pars = ARR("SetInt", sourceid);
    		return this.GetDataSet(query, pars);
    	}

    	///Get source by name.
        /// <param name="sourcename">Source name.</param>
        /// <returns>Resulting data set.</returns>
        public DataSet GetSourceByName(String sourcename) {
            if (sourcename == null || sourcename == "") return null;
    		var query = Strings.Concat("SELECT * FROM sources where s_SourceName = ?");
    		Object[] pars = ARR("SetString", sourcename);
    		return this.GetDataSet(query, pars);
    	}

        ///Check whether source exists.
        /// <param name="sourcename">Source name.</param>
        /// <returns>True if exists.</returns>
    	public Boolean CheckSourceName(String sourcename) {
        	return CheckSourceName(sourcename, null);
        }

        ///Check whether source exists.
        /// <param name="sourcename">Source name.</param>
        /// <param name="source">Source object (if found) copied to element 0 of object array.</param>
        /// <returns>True if exists.</returns>
        public Boolean CheckSourceName(String sourcename, Object[]source) {
    		var dsSources = this.EnumSources();
    		var source_found = false;
    		for (int n = 0; n < dsSources.GetSize(); n++) {
    			var oSource = dsSources.GetRow(n);
    			if (EQ(oSource["s_SourceName"], sourcename)) {
    				source_found = true;
    				if (source != null)
                        source[0] = oSource;
    				break;
    			}
    		}
    		return source_found;
    	}
    }
}