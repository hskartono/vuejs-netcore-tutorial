<template>
    <b-modal id="popup-error" scrollable 
        :no-close-on-backdrop=true 
        title="Error" 
        :no-close-on-esc=true 
        v-model="isShowPopup" 
        ok-title="Ok"
        ok-variant="light"
        @hidden="resetModal"
        header-bg-variant="warning"
        footer-bg-variant="warning"
        body-bg-variant="warning"
        size="md"
        :centered="true"
        :ok-only="true"
    >
     <b-card-text v-for="item in subtitle" v-bind:key="item.index">{{ item }}</b-card-text>
    </b-modal>
</template>
<style lang="scss" scoped>
  /deep/ .modal-dialog {
      max-width: 500px !important;
      max-height: 300px !important;
      margin-top: 4.75rem !important;
  }
</style>
<script>
export default {
    props: ["openPopup"],
    computed: {
        title() {
          return "Error Occured";
        },
        subtitle() {
          return this.$store.state.errorMessageGlobal;
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
          this.$store.dispatch("removeErrorMessageGlobal");
        },
    }
}
</script>