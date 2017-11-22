function GiveTip(e, msg) {
    if (!confirm(msg)) {
        if (e && e.preventDefault)
            e.preventDefault();
        else
            window.event.returnValue = false;
        return false;
    }
    return true;
}
//调用示例 onclientclick = "return GiveTip(event,'确认删除？')"   