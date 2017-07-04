var infobox = require('wiki-infobox');
var page = 'WHO (AM)';
var language = 'en';

infobox(page,language,function(err,data){
	var fs = require('fs');
	
	fs.writeFile("Sample.json", page + "\t" +JSON.stringify(data), function(err) {
    if(err) {
        return console.log(err);
    }

    console.log("The file was saved!");
	}); 
	
});