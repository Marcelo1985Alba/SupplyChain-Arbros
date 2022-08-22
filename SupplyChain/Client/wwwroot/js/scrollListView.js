function ListView(args) {
    // move the scroller to correspoding element by using scrollintoview method. 
    document.querySelector('[data-uid="' + args.id + '"]').scrollIntoView();
}