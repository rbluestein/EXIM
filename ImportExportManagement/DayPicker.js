var daypickers = [];

function DayPicker(obj_target) {
	this.popup    = dp_popup2;
	this.target = obj_target;
	this.id = daypickers.length;
	daypickers[this.id] = this;
}

function dp_popup2()  {
	var obj_dpwindow = window.open('DayPicker.aspx?id=' + this.id, 'DayPicker', 'width=340,height=140,status=no,resizable=no,top=200,left=200,dependent=yes,alwaysRaised=yes')
	obj_dpwindow.opener = window;
	obj_dpwindow.focus();
}