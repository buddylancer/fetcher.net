namespace Bula.Fetcher.Model {
    using System;

    using Bula.Model;

    /**
     * Manipulating with times.
     */
    public class DOTime : DOBase {

    	public DOTime (): base() {
    		this.table_name = "as_of_time";
    		this.id_field = "i_Id";
    	}
    }
}