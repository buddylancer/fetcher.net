namespace Bula.Fetcher.Controller.Testing {
    using System;

    using Bula.Fetcher;
    using Bula.Objects;
    using System.Collections;
    using Bula.Objects;

    public class CallMethod : Bula.Meta {
    	public static void Execute() {
    		Request.ExtractAllVars();

    		if (!Request.Contains("code"))
    			STOP("Code is required!");
    		String code = Request.Get("code");
    		if (!EQ(code, Config.SECURITY_CODE))
    			STOP("Incorrect code!");

    		if (!Request.Contains("package"))
    			STOP("Package is required!");
    		String package = Request.Get("package");
    		if (BLANK(package))
    			STOP("Empty package!");
    		package = Strings.Replace("-", "/", package);

    		if (!Request.Contains("class"))
    			STOP("Class is required!");
    		String class = Request.Get("class");
    		if (BLANK(class))
    			STOP("Empty class!");

    		if (!Request.Contains("method"))
    			STOP("Method is required!");
    		String method = Request.Get("method");
    		if (BLANK(method))
    			STOP("Empty method!");

    		String full_class = CAT(package, "/", class);
    		String class_file = CAT(full_class, ".class.php");
                    		if (doClass == null)
    			STOP("Can not instantiate class!");

    		int count = 0;
    		ArrayList pars = new ArrayList();
    		for (Intenger n = 1; n <= 6; n++) {
    			String par_name = CAT("par", n);
    			if (!Request.Contains(par_name))
    				break;
    			String par_value = Request.Get(par_name);
    			if (EQ(par_value, "_"))
    				par_value = "";
    			//pars_array[] = par_value;
                pars.Add(par_value);
    			count++;
    		}

            Ob_start();
            		if (result == null)
    			echo "null";
    		else {
                        }
            buffer = Str_replace(CAT(Config.LocalRoot), "_ROOT_", Ob_get_contents());
            Ob_end_clean();
            echo buffer;
    	}
    }

}