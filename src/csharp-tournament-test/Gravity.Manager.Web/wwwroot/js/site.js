$( document ).ready(function() {
    $(".utcToLocal").each(
        function (index, element) {
            // Convert UTC Unix timestamp to local date/time.            
            var unixTimestamp = parseInt(element.innerText);
            var offset = new Date().getTimezoneOffset() * 60000;
            var date = new Date(unixTimestamp - offset);
            element.innerText= date.toLocaleString();
        }
    );
});
