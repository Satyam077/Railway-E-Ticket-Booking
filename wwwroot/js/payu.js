window.submitPayuForm = function(html) {
    const div = document.createElement('div');
    div.innerHTML = html;
    document.body.appendChild(div);
    const form = div.querySelector('form');
    if (form) {
        form.submit();
    }
};
