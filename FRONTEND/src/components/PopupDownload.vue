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
    <b-card
      :title="isBusy ? contentLabel : (!isFailed ? 'Data telah siap' : 'Persiapan data gagal') "
      :sub-title="isBusy ? subTitle : ''"
    >
    <div class="text-center">
      <div v-if="isBusy">
        <b-spinner variant="success" label="Spinning"></b-spinner>
      </div>
      <div v-else-if="!isFailed">
        <b-button v-if="dataUrl" @click="downloadFile">Download File</b-button>
      </div>
    </div>
    </b-card>
    </b-modal>
</template>
<script>
import axios from 'axios';
import { saveAs } from 'file-saver';

export default {
    props: ["openPopup", "processId","label","resetModalMethod", "contentLabel"],
    data() {
      return {
        progressInterval : null,
        isBusy : false,
        isSuccess : false,
        isFailed : false,
        dataUrl : "",
        fileName : "",
      };
    },
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
    async mounted() {
      
    },
    watch : {
      processId() {
        this.checkProgress();
      }
    },
    methods: {
        resetModal() {
          this.$emit('resetModalMethod');
        },
        async downloadFile() {
          let this_ = this;
          this.$http.get(this.dataUrl, {responseType: 'blob'}).then(async response => {
              if (response != null) {
                  await saveAs(response.data, this_.fileName);
              }
          }).catch(error => {
              this.$store.dispatch('addErrorMessageGlobal', error);
              throw error;
          });
        },
        checkProgress() {
          this.isBusy = true;
          this.isFailed = false;
          this.progressInterval = setInterval(() => {
              this.$http.get(process.env.VUE_APP_API_URL + 'DownloadProcess/status/' + this.processId)
              .then(response => {
                if (response.data != null) {
                  if (response.data.status == "SUCCESS") {
                    this.dataUrl = response.data.filename;
                    this.fileName = this.GetFilename(this.dataUrl);
                    clearInterval(this.progressInterval);
                    this.isBusy = false;
                  }
                  else if (response.data.status == "FAILED") {
                    this.isFailed = true;
                    this.isBusy = false;
                  }
                } else {
                  clearInterval(this.progressInterval);
                  this.isBusy = false;
                }
              }).catch(error => {
                clearInterval(this.progressInterval);
                this.isBusy = false;
              })
          }, 3000);
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