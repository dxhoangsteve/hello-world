
function renderEditor(elementId) {
    var id = $(elementId).closest('form').find('[name="EditorUploadId"]').val();

    tinymce.init({
        selector: elementId,
        license_key: 'gpl',
        branding: false, menubar: false, resize: false, statusbar: false,
        //icons: "thin",
        //skin: "snow",
        plugins: "searchreplace autolink autosave directionality visualblocks visualchars fullscreen image link media codesample table charmap pagebreak advlist lists charmap emoticons",
        //plugins: "powerpaste casechange searchreplace autolink autosave directionality advcode visualblocks visualchars fullscreen image link media codesample table charmap pagebreak advlist lists checklist formatpainter charmap emoticons advtable",
        toolbar: "undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | image customImageButton forecolor backcolor removeformat casechange | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist checklist | customImageButton forecolor backcolor removeformat casechange | pagebreak | charmap emoticons codesample | link table | ltr rtl | fullscreen",
        //contextmenu: "",
        //height: "100%",
        //placeholder: "",
        //content_style: `@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans&display=swap');
        //                        body {
        //                            margin-left: calc(max((100% - 760px)/2, 0px)  + 40px);
        //                            margin-right: calc(max((100% - 760px)/2, 0px)  + 40px);
        //                            margin-top: 40px;
        //                            font-family: Plus Jakarta Sans;
        //                        }`,
        //font_family_formats: `Andale Mono=andale mono,times; Arial=arial,helvetica,sans-serif; Arial Black=arial black,avant garde; Book Antiqua=book antiqua,palatino;
        //                        Comic Sans MS=comic sans ms,sans-serif; Courier New=courier new,courier; Georgia=georgia,palatino; Helvetica=helvetica; Impact=impact,chicago;
        //                        Plus Jakarta Sans=Plus Jakarta Sans; Symbol=symbol; Tahoma=tahoma,arial,helvetica,sans-serif; Terminal=terminal,monaco; Times New Roman=times new roman,times; Trebuchet MS=trebuchet ms,geneva;
        //                        Verdana=verdana,geneva; Webdings=webdings; Wingdings=wingdings,zapf dingbats`,
        content_css: "/lib/tinymce-7/custom-tiny.css",
        images_upload_url: '/admin/editor/uploadimage?id=' + id, // id tạm
        images_reuse_filename: true
    });
}