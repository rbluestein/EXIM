var timepickers = [];

function TimePicker(obj_target) {
	this.popup    = tp_popup2;
	this.target = obj_target;
	this.id = timepickers.length;
	timepickers[this.id] = this;
}

function tp_popup2()  {
	var obj_tpwindow = window.open('TimePicker.aspx?id=' + this.id, 'TimePicker', 'width=465,height=140,status=no,resizable=no,top=200,left=200,dependent=yes,alwaysRaised=yes')
	obj_tpwindow.opener = window;
	obj_tpwindow.focus();
}