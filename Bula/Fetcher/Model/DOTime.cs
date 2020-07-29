namespace Bula.Fetcher.Model {
    using System;

    using Bula.Model;

    /// <summary>
    /// Manipulating with times.
    /// </summary>
    public class DOTime : DOBase {
        /// Public constructor (overrides base constructor) 
    	public DOTime (): base() {
    		this.table_name = "as_of_time";
    		this.id_field = "i_Id";
    	}
    }
}