<template>
    <b-modal id="my-modal" scrollable 
        :no-close-on-backdrop=true 
        :title="label" 
        :no-close-on-esc=true 
        v-model="isShowPopup" 
        ok-title="Ok" 
        ok-variant="success" 
        @hidden="resetModal"
    >
    <b-card>
      <pdf
			:src="pdfUrl"
			@num-pages="pageCount = $event"
			@page-loaded="currentPage = $event"
		></pdf>
    </b-card>
    <template #modal-footer="{ ok }">
      <b-button size="sm" variant="primary" @click="downloadPDF()">
        Download
      </b-button>
      <b-button size="sm" variant="success" class="float-right" @click="ok()">
        Ok
      </b-button>
    </template>
    </b-modal>
</template>
<style lang="scss" scoped>
/deep/.modal-footer {
  display: block;
}
</style>
<script>
import pdf from 'vue-pdf';
import PdfViewer from '@/components/PdfViewer';
import { saveAs } from 'file-saver';

export default {
    components: {pdf, PdfViewer},
    props: ["openPopup", "pdfUrl","label","resetModalMethod"],
    computed: {
        subTitle() {
            return "Process Id : " + this.processId;
        },
        isShowPopup : {
            get: function () {
              if (this.openPopup) return true;
              else return false;
            },
            set: function (newValue) {}
          }
    },
    methods: {
        resetModal() {
          this.$emit('resetModalMethod');
        },
        async downloadPDF() {
          let fileName = this.GetFilename(this.pdfUrl);
          await saveAs(this.pdfUrl, fileName);
        },
        GetFilename(url)
        {
          if (url)
          {
              var m = url.toString().match(/.*\/(.+?)\./);
              if (m && m.length > 1)
              {
                return m[1];
              }
          }
          return "";
        }
    }
}
</script>