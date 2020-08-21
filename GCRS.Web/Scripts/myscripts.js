function openNav() {
    document.getElementById("mySidenav").style.width = "50%";
}

function invNav() {
    if (document.getElementById("mySidenav").style.width == "50%") {
        document.getElementById("mySidenav").style.width = "0";
    }
    else {
        document.getElementById("mySidenav").style.width = "50%";
    }

    if (document.getElementById("mySidenavMap").style.width == "50%") {
        document.getElementById("mySidenavMap").style.width = "0";
    }
    else {
        document.getElementById("mySidenavMap").style.width = "50%";
    }
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}
function closeNavMap() {
    document.getElementById("mySidenavMap").style.width = "0";
}